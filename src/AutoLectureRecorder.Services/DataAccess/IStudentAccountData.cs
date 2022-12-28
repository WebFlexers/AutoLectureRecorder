using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;

public interface IStudentAccountData
{
    Task InsertStudentAccount(string registrationNumber, string academicEmailAddress, string password);
    Task DeleteStudentAccount();
    Task<ReactiveStudentAccount>? GetStudentAccount();
}