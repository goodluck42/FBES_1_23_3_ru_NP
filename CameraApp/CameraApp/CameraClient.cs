using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Documents;

namespace CameraApp;

public sealed class CameraClient
{
	private const int UdpDatagramSize = 65000;
	private TcpClient _tcpClient;
	private NetworkStream _tcpNetworkStream;
	private BinaryWriter _tcpBinaryWriter;
	private UdpClient _udpClient;
	private IPEndPoint _udpRemoteServerEndPoint = IPEndPoint.Parse("127.0.0.1:5001");

	public CameraClient()
	{
		_tcpClient = new TcpClient();
		_tcpClient.Connect(IPEndPoint.Parse($"127.0.0.1:5000"));
		_tcpNetworkStream = _tcpClient.GetStream();
		_tcpBinaryWriter = new BinaryWriter(_tcpNetworkStream);
		_udpClient = new UdpClient();
	}

	public void ListenForFrames()
	{
		
	}

	private readonly byte[] FrameBuffer = new byte[UdpDatagramSize];

	public void SendFrame(MemoryStream memoryStream)
	{
		var datagramCount = memoryStream.Length % UdpDatagramSize == 0
			? memoryStream.Length / UdpDatagramSize
			: memoryStream.Length / UdpDatagramSize + 1;

		_tcpBinaryWriter.Write(datagramCount);

		for (int i = 0; i < datagramCount; i++)
		{
			var read = memoryStream.Read(FrameBuffer);

			if (read == UdpDatagramSize)
			{
				_udpClient.Send(FrameBuffer, _udpRemoteServerEndPoint);
			}
			else
			{
				_udpClient.Send(FrameBuffer.AsSpan()[..read], _udpRemoteServerEndPoint);
			}
		}
	}

	public void SendFrame(byte[] frameBytes)
	{
		throw new NotImplementedException();
		// var datagramCount = frameBytes.Length % UdpDatagramSize == 0
		// 	? frameBytes.Length / UdpDatagramSize
		// 	: frameBytes.Length / UdpDatagramSize + 1;
		//
		// _tcpBinaryWriter.Write(datagramCount);
		//
		//
		//
		// for (int i = 0; i < datagramCount; i++)
		// {
		// 	var span = new ReadOnlySpan<byte>(frameBytes, );	
		// 	
		// 	// _udpClient.Send(frameBytes[(i * UdpDatagramSize)..(i != datagramCount - 1 ? UdpDatagramSize * (i + 1) : UdpDatagramSize ]);
		// }
	}
}