using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;

async Task Receive()
{
    UdpClient server = null!;
    try
    {
        server = new UdpClient(666);
        var myself = System.Text.Encoding.UTF8.GetBytes("Le prof");
        while (true)
        {
            var result = await server.ReceiveAsync();
            var payload = System.Text.Encoding.UTF8.GetString(result.Buffer);
            Console.WriteLine($"I received {payload} from {result.RemoteEndPoint}");
            await server.SendAsync(myself, myself.Length, result.RemoteEndPoint);
        }
    }
    finally
    {
        server?.Dispose();
    }
}

Console.OutputEncoding = Encoding.UTF8;

_ = Receive();

//using var client = new UdpClient();
//await client.SendAsync([1, 2, 3, 4], 4, "10.1.24.13", 666);
//var result = await client.ReceiveAsync();
//Console.WriteLine($"I received a datagram of {result.Buffer.Length} bytes from {result.RemoteEndPoint}");

Console.ReadLine();
