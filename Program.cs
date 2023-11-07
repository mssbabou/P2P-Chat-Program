namespace P2PChat
{
    class Program
    {
        static void Main(string[] args)
        {
            P2PChat chat = new P2PChat(IP: "0.0.0.0", port: 5000);
            chat.StartServer();

            if(args.Length > 0) chat.ConnectToPeer(args[0], 5000);

            while (true)
            {
                Console.Write("Enter message: ");
                string message = Console.ReadLine();

                if(message == "exit") break;

                chat.SendData(message);
            }
        }
    }
}