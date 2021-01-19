﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Auto_Lecture_Recorder.Youtube
{
    public class YoutubeUploader
    {
        UserCredential credential;
        string videoID;
        public Dictionary<string, Playlist> Playlists { get; set; } = new Dictionary<string, Playlist>();
        public bool Authenticated { get; set; }
        public bool CurrentlyUploading { get; set; } = false;
        public long CurrentProgress { get; set; } = 0;
        public long videoSize { get; set; } = 0;

        //more info about Task classes and methods:
        // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-5.0
        // https://stackoverflow.com/questions/16728338/how-can-i-run-both-of-these-methods-at-the-same-time-in-net-4-5

        public async Task Run(string VideoFilePath, string VideoName, string playlistName, string Description)
        {
            var videoUploadTask = Task.Run(() => uploadVideo(VideoFilePath, VideoName, playlistName, Description));
            await videoUploadTask;
            //var playlistVideoAddTask = Task.Run(() => playlistVideoAdder(videoID));
            // await playlistVideoAddTask;
        }
        public async Task<bool> Authenticate() //Authorization section
        {
            try
            {
                using (var stream = new FileStream("client_id3.json", FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        // This OAuth 2.0 access scope allows for full read/write access to the
                        // authenticated user's account.
                        new[] { YouTubeService.Scope.Youtube },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(this.GetType().ToString())
                    );
                }
                Authenticated = true;
            }
            catch
            {
                Authenticated = false;
            }

            return Authenticated;

        }
        private async void uploadVideo(string VideoFilePath, string VideoName, string playlistName, string Description)
        {
            //int counter = 0;
            //Path.Combine("client_id" + counter, ".json");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });


            if (await Authenticate())
            {
                //video creation
                var video = new Video();
                video.Snippet = new VideoSnippet();
                video.Snippet.Title = VideoName;
                video.Snippet.Description = Description;
                video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                video.Status = new VideoStatus();
                video.Status.PrivacyStatus = "private"; // or "private" or "public"
                var filePath = VideoFilePath; // Replace with path to actual movie file.

                //when it's Snippet is ready, call videoInsertRequest and upload it to youtube!
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    videoSize = fileStream.Length;
                    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived; //moving metadata to the videosInsertRequest_ResponseReceived() function.

                    CurrentlyUploading = true;

                    await videosInsertRequest.UploadAsync();
                }

                if (!Playlists.ContainsKey(playlistName))
                {
                    CreatePlaylist(playlistName);
                    try
                    {
                        Playlists[playlistName] = await youtubeService.Playlists.Insert(Playlists[playlistName], "snippet,status").ExecuteAsync();
                    }
                    catch
                    {
                        MessageBox.Show("Couldn't create playlist. If you manually deleted a playlist try manually recreating it");
                    }
                }



                try
                {

                    var newPlaylistItem = new PlaylistItem();
                    newPlaylistItem.Snippet = new PlaylistItemSnippet();

                    newPlaylistItem.Snippet.PlaylistId = Playlists[playlistName].Id;

                    newPlaylistItem.Snippet.ResourceId = new ResourceId();
                    newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                    newPlaylistItem.Snippet.ResourceId.VideoId = videoID;
                    newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
                }
                catch (Exception)
                {
                    MessageBox.Show("Video was not added to the playlist");
                }
            }

            void videosInsertRequest_ProgressChanged(IUploadProgress progress)
            {
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        Console.WriteLine(progress.BytesSent);
                        
                        CurrentProgress = (int)progress.BytesSent;
                        break;

                    case UploadStatus.Completed:
                        CurrentlyUploading = false;
                        break;

                    case UploadStatus.Failed:
                        MessageBox.Show(String.Format("An error prevented the upload from completing.\n{0}", progress.Exception));
                        break;
                }
            }

        }

        public bool UpdateProgressBar(System.Windows.Forms.ProgressBar bar)
        {
            if (videoSize != 0)
            {
                bar.Maximum = (int)videoSize;
                if (CurrentProgress <= bar.Maximum)
                    bar.Value = (int)CurrentProgress;
                else
                    bar.Value = bar.Maximum;
            }

            if (CurrentlyUploading)
                return false; 
            else
            {
                CurrentlyUploading = false;
                return true;
            }
                
        }

        public void CreatePlaylist(string name)
        {
            // Create a new, private playlist in the authorized user's channel.
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = name;
            newPlaylist.Snippet.Description = "";
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = "private";

            Playlists.Add(name, newPlaylist);
        }

        void videosInsertRequest_ResponseReceived(Video video) //receiving video.id
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            videoID = video.Id;
        }
            


    }

}

