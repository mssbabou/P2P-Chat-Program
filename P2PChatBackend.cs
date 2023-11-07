using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace P2PChat
{
    public class P2PChat
    {
        private IPAddress IP;
        private int port;
        private TcpListener server;
        private TcpClient client;

        public P2PChat(string IP = "127.0.0.1", int port = 5000)
        {
            this.IP = IPAddress.Parse(IP);
            this.port = port;
        }

        public void StartServer()
        {
            server = new TcpListener(IP, port);
            server.Start();
            Console.WriteLine($"Server started on {IP}:{port}.");

            Task.Run(AcceptClients);
        }

        private void AcceptClients()
        {
            try
            {
                while (true)
                {
                    client = server.AcceptTcpClient();
                    Console.WriteLine($"Connected to {client.Client.RemoteEndPoint}");

                    Task.Run(() => HandleClient(client));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e.Message}");
            }
            finally
            {
                server?.Stop();
            }
        }

        public void ConnectToPeer(string peerIP, int peerPort)
        {
            try
            {
                client = new TcpClient();
                client.Connect(IPAddress.Parse(peerIP), peerPort);
                Console.WriteLine($"Connected to peer at {peerIP}:{peerPort}.");

                Task.Run(() => HandleClient(client));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        public void SendData(string message)
        {
            if (client == null || !client.Connected)
            {
                Console.WriteLine("Not connected to any peer.");
                return;
            }

            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();

                stream.Write(byteData, 0, byteData.Length);

                Console.WriteLine("Sent: {0}", message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        public bool IsConnectionActive()
        {
            if (client == null || !client.Client.Connected)
                return false;

            bool isSocketConnected = true;
            try
            {
                isSocketConnected &= client.Client.Poll(0, SelectMode.SelectRead);
                isSocketConnected &= client.Client.Available == 0;
                if (client.Client.Poll(0, SelectMode.SelectWrite))
                {
                    byte[] buffer = new byte[1];
                    if (client.Client.Send(buffer, 0, SocketFlags.Peek) == 0)
                    {
                        isSocketConnected = false;
                    }
                }
            }
            catch
            {
                isSocketConnected = false;
            }

            return isSocketConnected;
        }


        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] bytes = new byte[256];
            string data = null;

            try
            {
                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
