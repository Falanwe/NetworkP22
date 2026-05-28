using System.Net;
using System.Net.Sockets;

var server = new TcpListener(IPAddress.Any, 667);
server.Start();

async Task Serve(CancellationToken token)
{

    while (!token.IsCancellationRequested)
    {
        try
        {
            _ = ServeSingleClient(await server.AcceptTcpClientAsync(token), token);
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == token)
        {
            // shutdown
        }
    }
}

async Task ServeSingleClient(TcpClient remoteClient, CancellationToken token)
{
    var buffer = new byte[1024];

    using (remoteClient)
    {
        var networkStream = remoteClient.GetStream();

        try
        {
            var readBytes = await networkStream.ReadAsync(buffer, token);
            Console.WriteLine($"I received {readBytes} bytes from {networkStream.Socket.RemoteEndPoint}");
            remoteClient.Close();
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == token)
        {
            //shutdown
        }
    }
}

using var cts = new CancellationTokenSource();

_ = Serve(cts.Token);

Console.WriteLine("Server is running. Press Enter to stop.");
Console.ReadLine();
cts.Cancel();

