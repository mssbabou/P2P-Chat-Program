using System.Text;

namespace P2PChat
{
    class Program
    {
        static void Main(string[] args)
        {
            P2PChat chat = new P2PChat(IP: args[0], port: int.Parse(args[1]));
            chat.StartServer();

            if(args.Length > 2) chat.ConnectToPeer(args[2], int.Parse(args[3]));

            chat.OnDataReceived += (sender, data) =>
            {
                Console.WriteLine($"Peer: {data}. Data length: {data.Length}");
            };

            while (true)
            {
                string? message = Console.ReadLine();

                if(string.IsNullOrEmpty(message)) continue;
                if(message == "exit") break;

                chat.SendDataToPeer(Encoding.ASCII.GetBytes(message));
            }
        }
    }
}