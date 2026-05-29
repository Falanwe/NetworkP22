using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronisation
{
    class SynchronisationClient
    {
        private readonly UdpClient _udpClient = new();
        public ushort ClientId { get; private set; }

        public event Action<ushort, Memory<byte>> UpdateReceived;

        public async Task Run(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync();
                var data = result.Buffer;
                var action = (ServerAction)data[0];
                switch (action)
                {
                    case ServerAction.SendClientId:
                        ClientId = BitConverter.ToUInt16(data, 1);
                        break;
                    case ServerAction.SendUpdate:
                        var clientId = BitConverter.ToUInt16(data, 1);
                        UpdateReceived?.Invoke(clientId, data.AsMemory(3));
                        break;
                }
            }
        }

        public void ConnectToServer(string serverHost, int serverPort)
        {
            _udpClient.Connect(serverHost, serverPort);
            _udpClient.Send(new byte[] { (byte)ClientAction.RegisterClient }, 1);
        }

        public void SendUpdate(ReadOnlySpan<byte> data)
        {
            byte[] buffer = null;
            try
            {
                buffer = ArrayPool<byte>.Shared.Rent(data.Length+1);
                buffer[0] = (byte)ClientAction.SendUpdate;
                data.CopyTo(buffer[1..^0]);
            }
            finally
            {
                if (buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            
        }
    }
}
