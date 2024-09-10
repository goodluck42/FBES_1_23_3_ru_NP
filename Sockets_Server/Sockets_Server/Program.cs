using System.Net;
using System.Net.Sockets;
using System.Text;


var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55555);

serverSocket.Bind(serverEndpoint);
serverSocket.Listen();

var clients = new List<Socket>();

while (true)
{
	var clientSocket = serverSocket.Accept();

	Console.WriteLine("Client connected");
	
	clients.Add(clientSocket);

	_ = Task.Run(() =>
	{
		Console.WriteLine("Listening a client...");
		try
		{
			var buffer = new byte[512];

			while (true)
			{
				clientSocket.Receive(buffer);

				Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer)}");

				Array.Clear(buffer);
			}
		}
		finally
		{
			clients.Remove(clientSocket);
		}
	});
}