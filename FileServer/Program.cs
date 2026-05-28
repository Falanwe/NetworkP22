using FileServer;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

var fileManager = new FileManager();

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

string GetIpAddress(EndPoint endPoint)
{
    if (endPoint is IPEndPoint ipEndPoint)
    {
        return ipEndPoint.Address.ToString();
    }
    else
    {
        throw new ArgumentException("Unsupported EndPoint type", nameof(endPoint));
    }
}

async Task<string> ReadFileName(byte[] buffer, NetworkStream networkStream, CancellationToken token)
{
    await networkStream.ReadExactlyAsync(buffer, 0, 1, token);
    var fileNameLength = buffer[0];
    await networkStream.ReadExactlyAsync(buffer, 0, fileNameLength, token);
    var fileName = System.Text.Encoding.UTF8.GetString(buffer, 0, fileNameLength);
    return fileName;
}

async Task ServeSingleClient(TcpClient remoteClient, CancellationToken token)
{
    var buffer = new byte[128];

    using (remoteClient)
    {
        var networkStream = remoteClient.GetStream();

        try
        {
            Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} connected");

            await networkStream.ReadExactlyAsync(buffer, 0, 1, token);
            switch ((Operation)buffer[0])
            {
                case Operation.Upload:
                    {
                        Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} wants to upload a file");
                        string fileName = await ReadFileName(buffer, networkStream, token);
                        await networkStream.ReadExactlyAsync(buffer, 0, 4, token);
                        var fileLength = BinaryPrimitives.ReadInt32BigEndian(buffer);
                        var currentUse = fileManager.GetMemoryUseForUser(GetIpAddress(networkStream.Socket.RemoteEndPoint));
                        if (currentUse + fileLength > 1024 * 1024)
                        {                           
                            Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} cannot upload {fileName} because it would exceed the 10MB limit (declared file size: {fileLength} bytes)");
                        }
                        else
                        {
                            var fileContent = new MemoryStream();
                            await networkStream.CopyToAsync(fileContent, fileLength, token);
                            fileManager.AddFile(GetIpAddress(networkStream.Socket.RemoteEndPoint), fileName, fileContent.ToArray());
                            Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} uploaded {fileName} ({fileLength} bytes)");
                        }
                        break;
                    }
                case Operation.Download:
                    {
                        Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} wants to download a file");
                        string fileName = await ReadFileName(buffer, networkStream, token);
                        if (fileManager.TryGetFile(fileName, out var fileContent))
                        {
                            var fileLength = fileContent.Length;
                            BinaryPrimitives.WriteInt32BigEndian(buffer, fileLength);
                            await networkStream.WriteAsync(buffer, 0, 4, token);
                            await networkStream.WriteAsync(fileContent, 0, fileLength, token);
                            Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} downloaded {fileName} ({fileLength} bytes)");
                        }
                        else
                        {
                            Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} requested {fileName} but it does not exist");
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"{networkStream.Socket.RemoteEndPoint} sent an unknown command");
                        break;
                    }
            }
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

Console.WriteLine("File server is running. Press Enter to stop.");
Console.ReadLine();
cts.Cancel();

