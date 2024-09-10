using System.Net;
using System.Net.Sockets;
using System.Text;

var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

clientSocket.Connect(IPAddress.Parse("127.0.0.1"), 55555);

while (true)
{
	string? input = Console.ReadLine();

	if (input is null or "0")
	{
		break;
	}
	
	clientSocket.Send(Encoding.ASCII.GetBytes(input));
}


