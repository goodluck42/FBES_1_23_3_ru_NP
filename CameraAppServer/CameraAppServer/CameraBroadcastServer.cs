using System.Net;
using System.Net.Sockets;

namespace CameraAppServer;

public sealed class CameraBroadcastServer
{
	private const string ListeningAddress = "127.0.0.1";
	private const int ListeningTcpPort = 5000;
	private const int ListeningUdpPort = 5001;
	private const int UdpDatagramPort = 5002;
	private const int DatagramSize = 65000;
	private const int TcpBufferSize = sizeof(int);

	private readonly List<TcpClient> _tcpClients;
	private readonly TcpListener _tcpListener;
	private readonly object _o = new();
	private readonly CancellationTokenSource _cts;
	private readonly UdpClient _udpClient;

	public event Action<IPEndPoint>? ClientConnected;
	public event Action<IPEndPoint>? ClientDisconnected;

	public CameraBroadcastServer()
	{
		_tcpListener = new TcpListener(IPEndPoint.Parse($"{ListeningAddress}:{ListeningTcpPort}"));
		_tcpClients = [];
		_cts = new();
		_udpClient = new UdpClient(IPEndPoint.Parse($"{ListeningAddress}:{ListeningUdpPort}"));
	}

	public void StartTcpServer()
	{
		_ = Task.Factory.StartNew(AcceptClient(), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
	}

	private Func<Task> AcceptClient()
	{
		return async () =>
		{
			while (!_cts.Token.IsCancellationRequested)
			{
				var tcpClient = await _tcpListener.AcceptTcpClientAsync();

				ClientConnected?.Invoke(tcpClient.Client.RemoteEndPoint.AsIPEndpoint()!);

				lock (_o)
				{
					_tcpClients.Add(tcpClient);
				}

				_ = Task.Factory.StartNew(ListenClient(tcpClient), TaskCreationOptions.LongRunning);
			}
		};
	}

	private Func<Task> ListenClient(TcpClient tcpClient)
	{
		return async () =>
		{
			var ipAddress = tcpClient.Client.RemoteEndPoint.GetIPAddress()!;
			var binaryReader = new BinaryReader(tcpClient.GetStream());

			try
			{
				var tcpBuffer = new byte[TcpBufferSize];
				var buffer = new byte[DatagramSize];
				var ipEndPoint = new IPEndPoint(ipAddress, UdpDatagramPort);
				var manualResetEvent = new ManualResetEventSlim(false);
				int datagramCount = -1;

				_ = Task.Run(() =>
				{
					try
					{
						while (!_cts.IsCancellationRequested)
						{
							_ = binaryReader.Read(buffer, 0, TcpBufferSize);
							manualResetEvent.Reset();
							datagramCount = BitConverter.ToInt32(tcpBuffer, 0);
							manualResetEvent.Set();
						}
					}
					finally
					{
						ClientDisconnected?.Invoke(tcpClient.Client.RemoteEndPoint.AsIPEndpoint()!);

						lock (_o)
						{
							_tcpClients.Remove(tcpClient);
						}
					}
				});

				while (true)
				{
					manualResetEvent.Wait();

					for (int i = 0; i < datagramCount && !manualResetEvent.IsSet; i++)
					{
						Array.Clear(buffer);
						await _udpClient.Client.ReceiveFromAsync(buffer, ipEndPoint);
						BroadcastFrameDatagram(buffer);
					}
				}
			}
			finally
			{
				lock (_o)
				{
					_tcpClients.Remove(tcpClient);
				}
			}
		};
	}

	private void BroadcastFrameDatagram(byte[] datagramBytes)
	{
		lock (_o)
		{
			foreach (var clientData in _tcpClients.Select(tcpClient => new
			         {
				         IPAddress = tcpClient.Client.RemoteEndPoint.GetIPAddress()!,
				         TcpClient = tcpClient
			         }))
			{
				_udpClient.Send(datagramBytes, new IPEndPoint(clientData.IPAddress, UdpDatagramPort));
			}
		}
	}
}