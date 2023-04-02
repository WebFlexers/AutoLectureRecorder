using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess.Interfaces;

public interface IStudentAccountRepository
{
    /// <summary>
    /// Gets the student account from the database asynchronously
    /// </summary>
    Task<ReactiveStudentAccount?> GetStudentAccountAsync();
    /// <summary>
    /// Insert a new student account to the database asynchronously
    /// </summary>
    Task<bool> InsertStudentAccountAsync(string registrationNumber, string academicEmailAddress, string password);
    /// <summary>
    /// Wipe out the student account table asynchronously
    /// </summary>
    Task<bool> DeleteStudentAccountAsync();
}