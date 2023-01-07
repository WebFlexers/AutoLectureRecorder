using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;

public interface IStudentAccountData
{
    Task InsertStudentAccountAsync(string registrationNumber, string academicEmailAddress, string password);
    Task DeleteStudentAccountAsync();
    Task<ReactiveStudentAccount>? GetStudentAccountAsync();
}