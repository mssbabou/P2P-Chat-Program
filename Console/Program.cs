using P2PChatCore;
using System.Text;

namespace P2PChatConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            P2PChat chat = new P2PChat(IP: "127.0.0.1", port: 5000);
            chat.StartServer();

            chat.OnDataReceived += (sender, data) =>
            {
                Console.WriteLine($"Peer: {data}. Data length: {data.Length}");
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to P2PChat!");
                Console.WriteLine();

                string[] mainMenuItems = new string[] { "Connect to peer", "Exit" };
                string mainMenuChoice = ConsoleManager.Start(mainMenuItems);
                if(mainMenuChoice == mainMenuItems[0])
                {
                    Console.Write("Enter IP address of peer: ");
                    string? ip = Console.ReadLine();
                    chat.ConnectToPeer(ip, 5000);

                    while (true)
                    {
                        string? message = Console.ReadLine();

                        if (string.IsNullOrEmpty(message)) continue;
                        if (message == "/exit") break;

                        chat.SendDataToPeer(Encoding.ASCII.GetBytes(message));
                    }

                }
                else if(mainMenuChoice == mainMenuItems[1])
                {
                    return;
                }

            }
        }
    }
}