using CameraAppServer;

var manualResetEvent = new ManualResetEventSlim(false);
var server = new CameraBroadcastServer();

server.StartTcpServer();
manualResetEvent.Wait();