using System.Net;
using System.Net.Sockets;
using System.Text;


bool IsSocketConnected(Socket socket)
{
    bool part1 = socket.Poll(1000, SelectMode.SelectRead);
    bool part2 = (socket.Available == 0);
    if ((part1 && part2) || !socket.Connected)
        return false;
    else
        return true;
}

while(true)
{
    try
    {
        Socket client = new Socket(IPAddress.Loopback.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Console.WriteLine("Press any Key to Connect to Server");
        Console.ReadKey();
        client.Connect(IPAddress.Loopback, 9000);

        Console.WriteLine("Socket connected to {0}",
                            client.RemoteEndPoint.ToString());

        string rawData;
        byte[] buffer;
        int bytesInBuffer;

        while (IsSocketConnected(client))
        {
            buffer = new byte[20];
            bytesInBuffer = client.Receive(buffer);

            rawData = Encoding.UTF8.GetString(buffer, 0, bytesInBuffer);
            if(rawData.Length > 0)
            {
                Console.Write(rawData);
            }
        }

        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
        Console.ForegroundColor= ConsoleColor.White;
    }
    Console.WriteLine("Disconnected");
}