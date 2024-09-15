global using System.Net;
global using System.Net.Sockets;
using System.Text;

var manualResetEvent = new ManualResetEvent(false);
var endPoint = IPEndPoint.Parse("172.16.0.201:8080");
// var endPoint = IPEndPoint.Parse("127.0.0.1:8080");
// 127.0.0.1 - localhost

var simpleMessenger = new SimpleMessenger(endPoint);

simpleMessenger.OnMessageReceived += (socket, message) =>
{
	if (socket.RemoteEndPoint is not IPEndPoint ipEndPoint)
	{
		Console.WriteLine("Cast Error");
		
		return;
	}
	
	Console.WriteLine($"[{ipEndPoint}]:[{DateTime.Now}]: {message}");
};

simpleMessenger.OnMessengerListening += () => Console.WriteLine("Listening...");
simpleMessenger.OnClientConnected += client => Console.WriteLine("A client connected!");
simpleMessenger.OnClientDisconnected += client => Console.WriteLine("A client disconnected!");

simpleMessenger.Start();

manualResetEvent.WaitOne();

public class SimpleMessenger
{
	private Socket _serverSocket;
	private List<Socket> _clients = new();
	private CancellationTokenSource _cts = new();
	private object _o = new();

	public SimpleMessenger(IPEndPoint endPoint)
	{
		_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		_serverSocket.Bind(endPoint);
	}
	
	public event Action<Socket, string>? OnMessageReceived;
	public event Action<Socket>? OnClientConnected;
	public event Action<Socket>? OnClientDisconnected;
	public event Action? OnMessengerListening;

	public void Start()
	{
		_serverSocket.Listen();

		Task.Factory.StartNew(AcceptClients,
			_cts.Token,
			TaskCreationOptions.LongRunning,
			TaskScheduler.Default);
		
		OnMessengerListening?.Invoke();
	}

	public void Stop()
	{
		_cts.Cancel();
		
		// ...
	}

	private void AcceptClients()
	{
		try
		{
			while (!_cts.Token.IsCancellationRequested)
			{
				var client = _serverSocket.Accept();

				OnClientConnected?.Invoke(client);

				lock (_o)
				{
					_clients.Add(client);	
				}
				
				Task.Factory.StartNew(() => AcceptMessages(client), _cts.Token,
					TaskCreationOptions.LongRunning,
					TaskScheduler.Default);
			}
		}
		finally
		{
			_serverSocket.Dispose();

			lock (_o)
			{
				foreach (var client in _clients)
				{
					client.Dispose();
				}
			}
		}
	}

	private void AcceptMessages(Socket client)
	{
		var buffer = new byte[1024];
		int bytesRead;

		try
		{
			//client.Receive(buffer);

			// var remoteEndPoint = client.RemoteEndPoint;
			//
			// _serverSocket.ReceiveFrom(buffer, ref remoteEndPoint);
			// _serverSocket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, client.RemoteEndPoint);
			
			while (!_cts.Token.IsCancellationRequested && (bytesRead = client.Receive(buffer)) > 0)
			{
				var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
				
				OnMessageReceived?.Invoke(client, message);
				
				Array.Clear(buffer);
			}
		}
		finally
		{
			client.Dispose();

			lock (_o)
			{
				_clients.Remove(client);
			}
			
			OnClientDisconnected?.Invoke(client);
		}
	}
}