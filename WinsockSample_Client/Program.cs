using System.Net;
using System.Net.Sockets;
using System.Text;

var remoteEndPoint = IPEndPoint.Parse("127.0.0.1:13370");
var mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

mySocket.Connect(remoteEndPoint);


while (true)
{
	var message = Console.ReadLine();

	if (message is null or "quit")
	{
		break;
	}

	mySocket.Send(Encoding.UTF8.GetBytes(message));
}