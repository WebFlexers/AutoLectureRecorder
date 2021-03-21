using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace YoutubeAPI
{
    public class YoutubeUploader
    {
        UserCredential credential;
        string videoID;
        bool authenticate = true;

        public static string CurrentDirectory { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                    "AutoLectureRecorder", "Youtube"); }
        public static bool IsAuthenticated 
        { 
            get 
            { 
                if (File.Exists(Path.Combine(CurrentDirectory, "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user")))
                    return true;
                else
                    return false;
            } 
        }

        //more info about Task classes and methods:
        // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-5.0
        // https://stackoverflow.com/questions/16728338/how-can-i-run-both-of-these-methods-at-the-same-time-in-net-4-5

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
                        new FileDataStore(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                    "AutoLectureRecorder", "Youtube"), true)
                    );
                }
                
            }
            catch { authenticate = false;   }

            return authenticate;

        }


        ProgressBar _progressBar;
        public async Task UploadVideo(string VideoFilePath, string VideoName, string Description, ProgressBar progressBar)
        {
            if (IsAuthenticated) {
                bool authenticate = await Authenticate();
            
                _progressBar = progressBar;

                var youtubeService = new YouTubeService(new BaseClientService.Initializer() //recieve authentication file
                {
                    HttpClientInitializer = credential,
                    ApplicationName = this.GetType().ToString()
                });

                //-----------Video Upload Section-------------------------------------------------
                var video = new Video();
                video.Snippet = new VideoSnippet();
                video.Snippet.Title = VideoName;
                video.Snippet.Description = Description;
                video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                video.Status = new VideoStatus();
                video.Status.PrivacyStatus = "private"; // or "private" or "public"
                var filePath = VideoFilePath; // Replace with path to actual movie file.

                //when video snippet is ready, call videoInsertRequest and upload it to youtube!
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    // Make progressbar max the video file size in bytes
                    Application.Current.Dispatcher.Invoke(() => _progressBar.Maximum = fileStream.Length);
                    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived; //moving metadata to the videosInsertRequest_ResponseReceived() function.
                    await videosInsertRequest.UploadAsync();
                }

                

                //-----------Playlist Section------------------------------------------------------
                // Define and execute the API request
                var request = youtubeService.Playlists.List("snippet,contentDetails");
                PlaylistListResponse response = new PlaylistListResponse();
                request.MaxResults = 25;
                request.Mine = true; //mine is true means that we are refering to our own channel, the one that is currently authenticated
                response = await request.ExecuteAsync(); //await response


                Trace.WriteLine(response.Items.Count);
                bool playlistIsFound = false;
                foreach (var playlist in response.Items)
                {
                    if (playlist.Snippet.Title.Contains(VideoName))
                    {
                        playlistIsFound = true;
                        var newPlaylistItem = new PlaylistItem();
                        newPlaylistItem.Snippet = new PlaylistItemSnippet();

                        newPlaylistItem.Snippet.PlaylistId = playlist.Id;

                        newPlaylistItem.Snippet.ResourceId = new ResourceId();
                        newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                        newPlaylistItem.Snippet.ResourceId.VideoId = videoID;
                        newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync(); //add video the playlist
                    }
                }

                if (playlistIsFound == false)
                {
                    //create a playlist
                    var newPlaylist = new Playlist();
                    newPlaylist.Snippet = new PlaylistSnippet();
                    newPlaylist.Snippet.Title = VideoName;
                    newPlaylist.Snippet.Description = "";
                    newPlaylist.Status = new PlaylistStatus();
                    newPlaylist.Status.PrivacyStatus = "private";
                    newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync(); //create a playlist

                    var newPlaylistItem = new PlaylistItem();
                    newPlaylistItem.Snippet = new PlaylistItemSnippet();
                    newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
                    newPlaylistItem.Snippet.ResourceId = new ResourceId();
                    newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                    newPlaylistItem.Snippet.ResourceId.VideoId = videoID;
                    newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync(); //add video to the created playlist
                }
            }
            else
            {
                throw new Exception("Rip");
            }
        }

        private void videosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    //MessageBox.Show(String.Format("{0} bytes sent.", progress.BytesSent));
                    UpdateProgressbar(progress);
                    break;

                case UploadStatus.Failed:
                    Trace.WriteLine(progress.Exception);
                    break;
                case UploadStatus.Completed:
                    UpdateProgressbar(progress);
                    break;
            }
        }

        private void UpdateProgressbar(IUploadProgress progress)
        {
            Trace.WriteLine("Entered UpdateProgressbar");
            Application.Current.Dispatcher.Invoke(() => 
            {
                if (_progressBar.Value >= progress.BytesSent)
                    _progressBar.Value = _progressBar.Maximum;
                else
                    _progressBar.Value = progress.BytesSent;
            });     
        }

        public async Task RetrievePlaylists()
        {
         bool  authenticate = await Authenticate();
            if (authenticate)
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = this.GetType().ToString()
                });

                // Define and execute the API request
                var request = youtubeService.Playlists.List("snippet,contentDetails");
                PlaylistListResponse response = new PlaylistListResponse();
                request.MaxResults = 25;
                request.Mine = true; //mine is true means that we are refering to our own channel
                response = await request.ExecuteAsync(); //await response

                Trace.WriteLine(response.Items.Count);

                foreach (var playlist in response.Items)
                {
                    //MessageBox.Show(playlist.Snippet.Title);
                    Trace.WriteLine(playlist.Id);
                }
            }else 
            { }
            // MessageBox.Show("Failed to Authenticate"); 
        
        }
        

        private void videosInsertRequest_ResponseReceived(Video video) //receiving video.id
        {
            videoID = video.Id;
        }

    }

}

