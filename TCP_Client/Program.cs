using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Bogus;

var manualResetEvent = new ManualResetEvent(false);
var tcpClient = new TcpClient();

tcpClient.Connect(IPEndPoint.Parse("172.16.0.201:9000"));

var networkStream = tcpClient.GetStream();
var binaryWriter = new BinaryWriter(networkStream);
var faker = new Faker<User>();

int i = 0;

var users = faker.RuleFor(a => a.Id, f => i++)
	.RuleFor(a => a.FirstName, f => f.Name.FirstName())
	.RuleFor(a => a.LastName, f => f.Name.LastName())
	.RuleFor(a => a.Age, f => f.Random.Int(18, 65)).Generate(3);

// var memoryStream = new MemoryStream();
// var binaryWriter = new BinaryWriter(memoryStream);

// BitConverter

foreach (var user in users)
{
	var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user));
	
	binaryWriter.Write(bytes);
	binaryWriter.Write(0xfd);
	binaryWriter.Flush();
}

manualResetEvent.WaitOne();

public class User
{
	public int Id { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public int Age { get; set; }
}

