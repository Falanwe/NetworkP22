using System.Net.Sockets;

async Task Receive()
{
    UdpClient server = null!;
    try
    {
        server = new UdpClient(666);
        while (true)
        {
            var result = await server.ReceiveAsync();
            Console.WriteLine($"I received a datagram of {result.Buffer.Length} bytes from {result.RemoteEndPoint}");
            await server.SendAsync(result.Buffer, result.Buffer.Length, result.RemoteEndPoint);
        }
    }
    finally
    {
        server?.Dispose();
    }
}

_ = Receive();

using var client = new UdpClient();
await client.SendAsync([1, 2, 3, 4], 4, "localhost", 666);
var result = await client.ReceiveAsync();
Console.WriteLine($"I received a datagram of {result.Buffer.Length} bytes from {result.RemoteEndPoint}");

Console.ReadLine();
