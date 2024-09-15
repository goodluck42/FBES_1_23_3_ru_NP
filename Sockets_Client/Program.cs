global using System.Net;
global using System.Net.Sockets;
using System.Text;

var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var endPoint = IPEndPoint.Parse("172.16.0.201:8080");

serverSocket.Connect(endPoint);

bool flag = true;

while (flag)
{
	var message = Console.ReadLine();

	if (message is null or "quit")
	{
		flag = false;
		
		continue;
	}
	
	serverSocket.Send(Encoding.UTF8.GetBytes(message));
}

serverSocket.Dispose();