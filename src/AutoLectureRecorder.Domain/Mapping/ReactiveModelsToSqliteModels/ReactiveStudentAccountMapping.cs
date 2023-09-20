using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;

public static class ReactiveStudentAccountMapping
{
    public static StudentAccount MapToSqliteModel(this ReactiveStudentAccount input)
    {
        return new StudentAccount(
            input.RegistrationNumber,
            input.EmailAddress,
            input.EncryptedPassword,
            input.Entropy);
    }
}