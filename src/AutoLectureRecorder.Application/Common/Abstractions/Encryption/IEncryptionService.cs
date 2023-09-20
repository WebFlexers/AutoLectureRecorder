namespace AutoLectureRecorder.Application.Common.Abstractions.Encryption;

public interface IEncryptionService
{
    string Encrypt(string input, byte[] entropy);
    string Decrypt(string encryptedData, byte[] entropy);
    byte[] GenerateRandomEntropy();
}