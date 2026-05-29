using SynchronisationServer;


using var cts = new CancellationTokenSource();
var server = new SyncServer();
_ = server.Serve(cts.Token);

Console.WriteLine("Sync server is running. Press Enter to stop.");
Console.ReadLine();
cts.Cancel();