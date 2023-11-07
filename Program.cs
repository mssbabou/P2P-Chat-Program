namespace P2PChat
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                P2PChat chat = new P2PChat(port: 5000);
                chat.StartServer();
            }else{
                P2PChat chat = new P2PChat(port: 5001);
                chat.StartServer();
                chat.ConnectToPeer("127.0.0.1", 5000);
                chat.SendData("Hello, world!");
            }

            Console.Read();
        }
    }
}