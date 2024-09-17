using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using UDP_Client;

//// using Socket
// var remoteEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
// var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//
// // #1
// // socket.Connect(remoteEndpoint);
// // socket.Send(Encoding.UTF8.GetBytes("Hello World123"));
//
// // #2
// //socket.SendTo(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()), remoteEndpoint);

//// using UdpClient

// var remoteEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
// var udpClient = new UdpClient();
//
// udpClient.Send(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()), remoteEndpoint);

//// Image transfer (UDP)
//
// var filePath = ShowDialog();
//
// if (string.IsNullOrEmpty(filePath))
// {
// 	return;
// }
//
// const int ChunkSize = 65000;
// const int BufferSize = ChunkSize + sizeof(int) * 2;
// using var fileStream = File.OpenRead(filePath);
// var remoteEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
// var udpClient = new UdpClient();
// var datagram = new byte[BufferSize];
// int datagramTotalCount = GetDatagramTotalCount((int)fileStream.Length, ChunkSize);
// int datagramNumber = 0;
// var datagramBytes = BitConverter.GetBytes(datagramTotalCount);
//
//
// while (fileStream.Position != fileStream.Length)
// {
// 	Console.WriteLine($"datagramNumber: {datagramNumber}");
// 	
// 	BitConverter.GetBytes(datagramNumber).CopyTo(datagram, 0);
// 	datagramBytes.CopyTo(datagram, 4);
// 	
// 	Console.WriteLine($"fileStream.Position: {fileStream.Position}");
// 	
// 	var bytesRead = fileStream.Read(datagram, sizeof(int) * 2, ChunkSize);
//
// 	Console.WriteLine($"read: {bytesRead}");
//
// 	if (bytesRead != ChunkSize)
// 	{
// 		udpClient.Send(datagram[..(bytesRead + sizeof(int) * 2)], remoteEndpoint);
// 	}
// 	else
// 	{
// 		udpClient.Send(datagram, remoteEndpoint);
// 	}
// 	
// 	
// 	Array.Clear(datagram);
// 	
// 	datagramNumber++;
// }
//
// Console.Read();

//// Image transfer (TCP)
var filePath = ShowDialog();

if (string.IsNullOrEmpty(filePath))
{
	return;
}

const int ChunkSize = 65000;
const int BufferSize = ChunkSize + sizeof(int) * 2;
using var fileStream = File.OpenRead(filePath);
var remoteEndpoint = IPEndPoint.Parse("127.0.0.1:8080");
var tcpClient = new TcpClient();
var datagram = new byte[BufferSize];
int datagramTotalCount = GetDatagramTotalCount((int)fileStream.Length, ChunkSize);
int datagramNumber = 0;
var datagramBytes = BitConverter.GetBytes(datagramTotalCount);

tcpClient.Connect(remoteEndpoint);

while (fileStream.Position != fileStream.Length)
{
	Console.WriteLine($"datagramNumber: {datagramNumber}");
	
	BitConverter.GetBytes(datagramNumber).CopyTo(datagram, 0);
	datagramBytes.CopyTo(datagram, 4);
	
	Console.WriteLine($"fileStream.Position: {fileStream.Position}");
	
	var bytesRead = fileStream.Read(datagram, sizeof(int) * 2, ChunkSize);

	Console.WriteLine($"read: {bytesRead}");

	if (bytesRead != ChunkSize)
	{
		tcpClient.Client.Send(datagram[..(bytesRead + sizeof(int) * 2)]);
	}
	else
	{
		tcpClient.Client.Send(datagram);
	}
	
	Array.Clear(datagram);
	
	datagramNumber++;
}

Console.Read();


int GetDatagramTotalCount(int totalBytes, int chunkSize)
{
	var chunks = totalBytes / chunkSize;

	return totalBytes % chunkSize == 0 ? chunks : chunks + 1;
}

public partial class Program
{
	// From https://www.pinvoke.net/default.aspx/comdlg32/GetOpenFileName.html
	[DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern bool GetOpenFileName(ref OpenFileName ofn);

	public static string ShowDialog()
	{
		var ofn = new OpenFileName();
		
		ofn.lStructSize = Marshal.SizeOf(ofn);
		ofn.lpstrFilter = "JPEG Files (*.jpg)\0*.jpg";
		ofn.lpstrFile = new string(new char[256]);
		ofn.nMaxFile = ofn.lpstrFile.Length;
		ofn.lpstrFileTitle = new string(new char[64]);
		ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
		ofn.lpstrTitle = "Open File Dialog...";
		
		if (GetOpenFileName(ref ofn))
		{
			return ofn.lpstrFile;
		}

		return string.Empty;
	}
}