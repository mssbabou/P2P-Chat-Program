using System.Security.Cryptography;

namespace P2PChatCore
{
    public static class EncryptionService
    {
        public static byte[] EncryptData(byte[] dataToEncrypt, string publicKey)
        {
            using RSA rsa = RSA.Create();
            rsa.FromXmlString(publicKey);

            byte[] encryptedData;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                encryptedData = EncryptWithAes(dataToEncrypt, aesAlg);

                // Clear the AES key from memory
                Array.Clear(aesAlg.Key, 0, aesAlg.Key.Length);

                byte[] encryptedAesKey = rsa.Encrypt(aesAlg.Key, RSAEncryptionPadding.OaepSHA256);

                return Combine(aesAlg.IV, encryptedAesKey, encryptedData);
            }
        }

        public static byte[] DecryptData(byte[] dataToDecrypt, string privateKey)
        {
            using RSA rsa = RSA.Create();
            rsa.FromXmlString(privateKey);

            (byte[] iv, byte[] encryptedKey, byte[] encryptedData) = Separate(dataToDecrypt, rsa.KeySize / 8);

            byte[] decryptedAesKey = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = decryptedAesKey;
            aesAlg.IV = iv;

            // Clear the AES key from memory
            Array.Clear(decryptedAesKey, 0, decryptedAesKey.Length);

            return DecryptWithAes(encryptedData, aesAlg);
        }

        private static byte[] EncryptWithAes(byte[] data, Aes aesAlg)
        {
            using ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        private static byte[] DecryptWithAes(byte[] data, Aes aesAlg)
        {
            using ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            return decryptor.TransformFinalBlock(data, 0, data.Length);
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
            ArraySegment<byte> iv = new ArraySegment<byte>(combinedData, 0, 16);
            ArraySegment<byte> encryptedKey = new ArraySegment<byte>(combinedData, iv.Count, rsaKeySizeInBytes);
            ArraySegment<byte> encryptedData = new ArraySegment<byte>(combinedData, iv.Count + encryptedKey.Count, combinedData.Length - iv.Count - encryptedKey.Count);

            return (iv.ToArray(), encryptedKey.ToArray(), encryptedData.ToArray());
        }
    }
}