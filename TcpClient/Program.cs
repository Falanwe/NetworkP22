using System.Net.Sockets;

using (var client = new TcpClient())
{
    await client.ConnectAsync("localhost", 667);
    await client.GetStream().WriteAsync([1, 2, 3, 4], 0, 4);
    client.Close();
}

Console.WriteLine("I sent 4 bytes to the server");
Console.ReadLine();