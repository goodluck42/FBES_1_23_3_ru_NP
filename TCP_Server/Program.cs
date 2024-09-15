using System.Net;
using System.Net.Sockets;
using System.Text;

var manualResetEvent = new ManualResetEvent(false);
var listener = new TcpListener(IPEndPoint.Parse("172.16.0.201:9000"));

listener.Start();

var tcpClient = listener.AcceptTcpClient();

using var networkStream = tcpClient.GetStream();
using var binaryReader = new BinaryReader(networkStream);

var bytes = new List<byte>();

try
{
	while (true)
	{
		byte b = binaryReader.ReadByte();

		if (b == 0xfd)
		{
			Console.WriteLine(Encoding.UTF8.GetString(bytes.ToArray()));

			bytes.Clear();
		}
		
		bytes.Add(b);

	}
}
catch (Exception e)
{
	Console.WriteLine(e.Message);
}

manualResetEvent.WaitOne();