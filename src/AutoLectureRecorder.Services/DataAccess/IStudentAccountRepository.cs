using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;

public interface IStudentAccountRepository
{
    /// <summary>
    /// Insert a new student account to the database asynchronously
    /// </summary>
    Task InsertStudentAccountAsync(string registrationNumber, string academicEmailAddress, string password);
    /// <summary>
    /// Gets the student account from the database asynchronously
    /// </summary>
    Task<ReactiveStudentAccount?> GetStudentAccountAsync();
    /// <summary>
    /// Wipe out the student account table asynchronously
    /// </summary>
    Task DeleteStudentAccountAsync();
}