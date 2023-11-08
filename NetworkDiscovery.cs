using System.Net;
using System.Net.Sockets;
using System.Text;

public static class NetworkDiscovery
{
    private static int discoveryPort = 8888;
    private static UdpClient udpClient = new UdpClient(discoveryPort);
    private static List<string> devices = new List<string>();

    // This static method starts listening for discovery responses.
    public static void StartListening()
    {
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    // This static method sends out a broadcast to discover devices.
    public static void DiscoverDevices()
    {
        var endPoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);
        string message = "Discovery: Request";
        byte[] sendBytes = Encoding.ASCII.GetBytes(message);
        udpClient.EnableBroadcast = true;
        udpClient.Send(sendBytes, sendBytes.Length, endPoint);
    }

    // This static callback handles incoming responses.
    private static void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, discoveryPort);
        byte[] bytes = udpClient.EndReceive(ar, ref groupEP);
        string receivedData = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

        lock (devices) // Synchronize access to the devices list.
        {
            if (!devices.Contains(receivedData))
            {
                devices.Add(receivedData);
            }
        }

        // Continue listening for broadcast messages.
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    // This static method returns the current list of discovered devices.
    public static List<string> GetDiscoveredDevices()
    {
        lock (devices) // Synchronize access to the devices list.
        {
            return new List<string>(devices); // Return a copy of the list.
        }
    }
}
