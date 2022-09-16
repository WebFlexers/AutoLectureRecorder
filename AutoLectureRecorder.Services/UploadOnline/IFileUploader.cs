namespace AutoLectureRecorder.Services.UploadOnline;

public interface IFileUploader
{
    void Authenticate(string username, string password);
    void UploadFile(string filePath, string fileName, string description);
}
