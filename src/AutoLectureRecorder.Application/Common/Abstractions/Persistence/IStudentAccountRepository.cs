using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.Common.Abstractions.Persistence;

public interface IStudentAccountRepository
{
    /// <summary>
    /// Gets the student account from the database asynchronously
    /// </summary>
    Task<ReactiveStudentAccount?> GetStudentAccount();

    /// <summary>
    /// Insert a new student account to the database asynchronously
    /// </summary>
    Task<bool> InsertStudentAccount(string registrationNumber, string academicEmailAddress, string encryptedPassword, 
        byte[] entropy);

    /// <summary>
    /// Wipe out the student account table asynchronously
    /// </summary>
    Task<bool> DeleteStudentAccount();
}