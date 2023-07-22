using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;

public static class StudentAccountMapping
{
    public static ReactiveStudentAccount MapToReactive(this StudentAccount input)
    {
        return new ReactiveStudentAccount(
            input.RegistrationNumber,
            input.EmailAddress,
            input.Password);
    }
}