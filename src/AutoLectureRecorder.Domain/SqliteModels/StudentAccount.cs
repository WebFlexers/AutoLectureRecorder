namespace AutoLectureRecorder.Domain.SqliteModels;

public record StudentAccount
(
    string RegistrationNumber,
    string EmailAddress,
    string? EncryptedPassword,
    byte[]? Entropy
)
{
    public StudentAccount() : this(default!, default!, default!, default!)
    { }
};
