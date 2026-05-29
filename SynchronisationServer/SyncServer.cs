using Synchronisation;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SynchronisationServer
{
    internal class SyncServer : IDisposable
    {
        private bool disposedValue;

        private readonly UdpClient _server = new(668);
        private int _nextClientId = 0;
        private Dictionary<IPEndPoint, ushort> _clientIds = [];

        public async Task Serve(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await _server.ReceiveAsync(token);
                    switch ((ClientAction)result.Buffer[0])
                    {
                        case ClientAction.RegisterClient:
                            {
                                var clientId = (ushort)Interlocked.Increment(ref _nextClientId);
                                lock (_clientIds)
                                {
                                    _clientIds[result.RemoteEndPoint] = clientId;
                                }
                                using var rent = MemoryPool<byte>.Shared.Rent(3);
                                var span = rent.Memory.Span;
                                span[0] = (byte)ServerAction.SendClientId;
                                BitConverter.TryWriteBytes(span[1..3], clientId);
                                await _server.SendAsync(rent.Memory, result.RemoteEndPoint, token);
                                break;
                            }
                        case ClientAction.SendUpdate:
                            {
                                IPEndPoint[] endpoints;
                                ushort currentClientId;
                                lock (_clientIds)
                                {
                                    if (!_clientIds.TryGetValue(result.RemoteEndPoint, out currentClientId))
                                    {
                                        Console.WriteLine("Received update from unregistered client!");
                                        break;
                                    }
                                    endpoints = _clientIds.Keys.Where(ep => ep != result.RemoteEndPoint).ToArray();
                                }
                                using (var memoryOwner = MemoryPool<byte>.Shared.Rent(result.Buffer.Length + 2))
                                {
                                    var memoryToSend = memoryOwner.Memory;
                                    var span = memoryToSend.Span;
                                    span[0] = (byte)ServerAction.SendUpdate;
                                    BitConverter.TryWriteBytes(span[1..3], currentClientId);
                                    result.Buffer.AsSpan().Slice(1).CopyTo(span.Slice(3));
                                    await Task.WhenAll(endpoints.Select(async ep => await _server.SendAsync(memoryToSend, ep, token)));
                                }
                                break;
                            }
                        default:
                            Console.WriteLine("Unknown client action!");
                            break;
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == token)
                {
                    // shutdown
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                _server?.Dispose();

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SyncServer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
