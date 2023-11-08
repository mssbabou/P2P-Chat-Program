using System.Security.Cryptography;
using System.Text;

public class EncryptionService
{
    public static byte[] EncryptData(byte[] dataToEncrypt, RSA rsa)
    {
        // Encrypt data with RSA public key
        return rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
    }

    public static byte[] DecryptData(byte[] dataToDecrypt, RSA rsa)
    {
        // Decrypt data with RSA private key
        return rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
    }
}