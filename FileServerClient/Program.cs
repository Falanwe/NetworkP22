using System.Buffers.Binary;

var tcpClient = new System.Net.Sockets.TcpClient();

await tcpClient.ConnectAsync("localhost", 667);
var stream = tcpClient.GetStream();

var buffer = new byte[4];
buffer[0] = 0;
await stream.WriteAsync(buffer, 0, 1);
var fileName = "duck.txt";
var fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
buffer[0] = (byte)fileNameBytes.Length;
await stream.WriteAsync(buffer, 0, 1);
await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

await stream.ReadExactlyAsync(buffer);
var length = BinaryPrimitives.ReadInt32BigEndian(buffer);

var fileContent = new byte[length];
await stream.ReadExactlyAsync(fileContent);

Console.ReadLine();