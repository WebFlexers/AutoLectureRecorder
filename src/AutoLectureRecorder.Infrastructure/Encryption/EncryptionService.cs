using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using AutoLectureRecorder.Application.Common.Abstractions.Encryption;

namespace AutoLectureRecorder.Infrastructure.Encryption;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class EncryptionService : IEncryptionService
{
    public string Encrypt(string input, byte[] entropy)
    {
        byte[] data = Encoding.Unicode.GetBytes(input);
        byte[] encrypted = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }
    
    public string Decrypt(string encryptedData, byte[] entropy)
    {
        byte[] data = Convert.FromBase64String(encryptedData);
        byte[] decrypted = ProtectedData.Unprotect(data, entropy, DataProtectionScope.CurrentUser);
        return Encoding.Unicode.GetString(decrypted);
    }
    
    public byte[] GenerateRandomEntropy()
    {
        int length = 32;
        
        var entropy = new byte[length];
        RandomNumberGenerator.Fill(entropy);
        return entropy;
    }
}