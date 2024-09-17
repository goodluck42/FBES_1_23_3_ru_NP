using System.Net;
using System.Net.Sockets;
using System.Text;

// // using Socket
// var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
// var myEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
//
// socket.Bind(myEndpoint);
//
// var datagram = new byte[65507];
//
// while (true)
// {
// 	// IPAddress.Any - 0.0.0.0
// 	var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
// 	var endPoint = (EndPoint)ipEndPoint;
//
// 	int bytesReceived = socket.ReceiveFrom(datagram, ref endPoint);
//
// 	if (bytesReceived > 0)
// 	{
// 		Console.WriteLine($"{Encoding.UTF8.GetString(datagram, 0, bytesReceived)}");
// 		Array.Clear(datagram);
// 	}
// }

//// using UdpClient

// var myEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
// var udpClient = new UdpClient(myEndpoint);
//
// // udpClient.Client.ReceiveBufferSize = 65507;
//
// while (true)
// {
// 	var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
// 	var datagram = udpClient.Receive(ref ipEndPoint);
//
// 	if (datagram.Length > 0)
// 	{
// 		Console.WriteLine($"{Encoding.UTF8.GetString(datagram, 0, datagram.Length)}");
// 		Array.Clear(datagram);
// 	}
// }

//// Image transfer (UDP)

// var myEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
// var udpClient = new UdpClient(myEndpoint);
// using var fileStream = File.Create("image.jpg");
//
// while (true)
// {
// 	var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
// 	var datagram = udpClient.Receive(ref ipEndPoint);
//
// 	if (datagram.Length > 0)
// 	{
// 		var datagramObject = new Datagram(datagram[(sizeof(int) * 2)..], BitConverter.ToInt32(datagram, 0));
// 		
// 		Console.WriteLine($"datagram.Length: {datagram.Length}");
// 		Console.WriteLine($"datagramObject.Length: {datagramObject.Length}");
// 		Console.WriteLine($"datagramObject.DatagramNumber: {datagramObject.DatagramNumber}");
// 		Console.WriteLine($"TotalDatagrams: {BitConverter.ToInt32(datagram[4..8])}");
// 		
// 		fileStream.Write(datagramObject.Bytes);
// 		fileStream.Flush();
// 	}
// }

//// Image transfer (TCP)

var myEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
var tcpListener = new TcpListener(myEndpoint);
using var fileStream = File.Create("image.jpg");

tcpListener.Start();

var client = tcpListener.AcceptTcpClient();
var buffer = new byte[65008];


while (true)
{
	var readBytes = client.Client.Receive(buffer);

	if (readBytes > 0)
	{
		var datagramObject = new Datagram(buffer[(sizeof(int) * 2)..], BitConverter.ToInt32(buffer, 0));
		
		Console.WriteLine($"datagram.Length: {buffer.Length}");
		Console.WriteLine($"datagramObject.Length: {datagramObject.Length}");
		Console.WriteLine($"datagramObject.DatagramNumber: {datagramObject.DatagramNumber}");
		Console.WriteLine($"TotalDatagrams: {BitConverter.ToInt32(buffer[4..8])}");
		
		fileStream.Write(datagramObject.Bytes);
		fileStream.Flush();
		
		Array.Clear(buffer);
	}
}

public sealed record Datagram(byte[] Bytes, int DatagramNumber)
{
	public int Length => Bytes.Length;
}