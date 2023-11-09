using System.Security.Cryptography;

public static class EncryptionService
{
    public static byte[] EncryptData(byte[] dataToEncrypt, string publicKey)
    {
        using RSA rsa = RSA.Create();
        rsa.FromXmlString(publicKey);

        byte[] aesIV;
        byte[] encryptedAesKey;
        byte[] encryptedData;

        using Aes aesAlg = Aes.Create();
        aesAlg.GenerateKey();
        aesAlg.GenerateIV();

        // Use RSA with OAEP-SHA256 padding
        encryptedAesKey = rsa.Encrypt(aesAlg.Key, RSAEncryptionPadding.OaepSHA256);
        aesIV = aesAlg.IV;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        encryptedData = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);

        return Combine(aesIV, encryptedAesKey, encryptedData);
    }

    public static byte[] DecryptData(byte[] dataToDecrypt, string privateKey)
    {
        using RSA rsa = RSA.Create();
        rsa.FromXmlString(privateKey);

        (byte[] iv, byte[] encryptedKey, byte[] encryptedData) = Separate(dataToDecrypt, rsa.KeySize / 8);

        // Use RSA with OAEP-SHA256 padding
        byte[] decryptedAesKey = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);

        using Aes aesAlg = Aes.Create();
        aesAlg.Key = decryptedAesKey;
        aesAlg.IV = iv;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
    }

    private static byte[] Combine(byte[] iv, byte[] encryptedKey, byte[] encryptedData)
    {
        byte[] combinedData = new byte[iv.Length + encryptedKey.Length + encryptedData.Length];
        Buffer.BlockCopy(iv, 0, combinedData, 0, iv.Length);
        Buffer.BlockCopy(encryptedKey, 0, combinedData, iv.Length, encryptedKey.Length);
        Buffer.BlockCopy(encryptedData, 0, combinedData, iv.Length + encryptedKey.Length, encryptedData.Length);
        return combinedData;
    }

    private static (byte[] iv, byte[] encryptedKey, byte[] encryptedData) Separate(byte[] combinedData, int rsaKeySizeInBytes)
    {
        byte[] iv = new byte[16];
        byte[] encryptedKey = new byte[rsaKeySizeInBytes];
        byte[] encryptedData = new byte[combinedData.Length - iv.Length - encryptedKey.Length];

        Buffer.BlockCopy(combinedData, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(combinedData, iv.Length, encryptedKey, 0, encryptedKey.Length);
        Buffer.BlockCopy(combinedData, iv.Length + encryptedKey.Length, encryptedData, 0, encryptedData.Length);

        return (iv, encryptedKey, encryptedData);
    }
}