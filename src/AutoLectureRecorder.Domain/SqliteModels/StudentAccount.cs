namespace AutoLectureRecorder.Domain.SqliteModels;

public record StudentAccount
(
    string RegistrationNumber,
    string EmailAddress,
    string Password
);
