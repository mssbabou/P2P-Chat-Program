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

        private IPAddress peerIP;
        private int peerPort;

        private TcpListener server;
        private TcpClient client;

        public event EventHandler<byte[]> OnDataReceived;

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

                    peerIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                    peerPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port;

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

                this.peerIP = IPAddress.Parse(peerIP);
                this.peerPort = peerPort;

                Task.Run(() => HandleClient(client));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        public void SendDataToPeer(byte[] data)
        {
            if (client == null || !client.Client.Connected)
            {
                Console.WriteLine("Not connected to any peer.");
                return;
            }

            try
            {
                NetworkStream stream = client.GetStream();

                // Send the length of the data as the first 4 bytes (32 bits)
                var dataLength = BitConverter.GetBytes(data.Length);
                stream.Write(dataLength, 0, dataLength.Length);

                // Then send the actual data
                stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }
        private int ReadFullMessage(NetworkStream stream, byte[] buffer, int length)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < length)
            {
                int bytesRead = stream.Read(buffer, totalBytesRead, length - totalBytesRead);
                if (bytesRead == 0)
                {
                    // The client disconnected or the stream was closed
                    break;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            try
            {
                while (true)
                {
                    // Read the length of the incoming data
                    byte[] lengthBytes = new byte[4];
                    int bytesRead = stream.Read(lengthBytes, 0, 4);
                    if (bytesRead == 0)
                        break; // The client disconnected.

                    int dataLength = BitConverter.ToInt32(lengthBytes, 0);
                    // Now read the actual data
                    byte[] bytes = new byte[dataLength];
                    bytesRead = ReadFullMessage(stream, bytes, dataLength);
                    if (bytesRead == dataLength)
                    {
                        OnDataReceived?.Invoke(this, bytes);
                    }
                    else
                    {
                        // Handle the case where the expected number of bytes was not received
                        Console.WriteLine("Received incomplete message.");
                        break;
                    }
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