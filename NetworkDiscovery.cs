using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public static class NetworkDiscovery
{
    private static readonly int discoveryPort = 8888;
    private static UdpClient udpClient;
    private static List<string> peers = new List<string>();

    static NetworkDiscovery()
    {
        udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.ExclusiveAddressUse = false;
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, discoveryPort));
    }

    public static void StartListening()
    {
        try
        {
            udpClient.BeginReceive(ReceiveCallback, null);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e.Message}");
        }
    }

    public static void DiscoverDevices()
    {
        try
        {
            var endPoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);
            string message = "Who is out there?";  // A more realistic discovery message
            byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            udpClient.EnableBroadcast = true;
            udpClient.Send(sendBytes, sendBytes.Length, endPoint);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e.Message}");
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, discoveryPort);
            byte[] bytes = udpClient.EndReceive(ar, ref groupEP);
            string receivedData = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

            // Here you might want to parse the receivedData to extract meaningful information.
            string deviceIdentifier = receivedData;  // For this example, we assume the entire message is the identifier.

            lock (peers)
            {
                if (!peers.Contains(deviceIdentifier))
                {
                    peers.Add(deviceIdentifier);
                }
            }

            udpClient.BeginReceive(ReceiveCallback, null);
        }
        catch (ObjectDisposedException)
        {
            // The socket was closed, ignore this exception.
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e.Message}");
        }
    }

    public static List<string> GetDiscoveredDevices()
    {
        lock (peers)
        {
            return new List<string>(peers);
        }
    }

    public static void StopListening()
    {
        udpClient?.Close();
        udpClient = null;
    }
}
