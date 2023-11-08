using System.Security.Cryptography;
using System.Text;

namespace P2PChat
{
    class Program
    {
        static void Main(string[] args)
        { 
            // Generate RSA keys
            var rsa = new RSACryptoServiceProvider();
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);

            var rsa2 = new RSACryptoServiceProvider();
            rsa2.FromXmlString(publicKey);

            // Original data to encrypt
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes("Secret Data");

            // Encrypt the data using the RSA public key
            byte[] encryptedData = EncryptionService.EncryptData(dataToEncrypt, rsa2);

            // Normally the encrypted data would be transmitted to the recipient at this point

            // Decrypt the data using the RSA private key
            byte[] decryptedText = EncryptionService.DecryptData(encryptedData, rsa);

            // Output the decrypted data
            Console.WriteLine($"Decrypted text: {decryptedText}");

            // Dispose the RSA instance
            rsa.Dispose();

            return;
            if(args.Length > 1)
            {
                NetworkDiscovery.StartListening();
                Thread.Sleep(1000);
                List<string> devices = NetworkDiscovery.GetDiscoveredDevices();
                foreach (string device in devices)
                {
                    Console.WriteLine(device);
                }
            }
            else
            {
                while(true)
                {
                    NetworkDiscovery.DiscoverDevices();
                    Thread.Sleep(1000);
                }
            }
            return;
            P2PChat chat = new P2PChat(IP: "0.0.0.0", port: 5002);
            chat.StartServer();

            if(args.Length > 0) chat.ConnectToPeer(args[0], 5000);

            while (true)
            {
                Console.Write("> ");
                string message = Console.ReadLine();

                if(string.IsNullOrEmpty(message)) continue;
                if(message == "exit") break;

                chat.SendDataToPeer(Encoding.ASCII.GetBytes(message));
            }
        }
    }
}