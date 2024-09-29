using System.Net;
using System.Security.Authentication;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

var myAddress = "skiba_al@itstep.edu.az";
using var smtpClient = new SmtpClient
{
	SslProtocols = SslProtocols.Tls13
};

smtpClient.MessageSent += (sender, eventArgs) => { Console.WriteLine($"Message sent: {eventArgs.Response}"); };

await smtpClient.ConnectAsync("smtp-mail.outlook.com", 587);

await smtpClient.AuthenticateAsync(new NetworkCredential
{
	UserName = myAddress,
	Password = File.ReadAllText("password")
});

var message = new MimeMessage();
var sender = new MailboxAddress(string.Empty, myAddress);

message.To.Add(new MailboxAddress(string.Empty, "valie_gi50@itstep.edu.az"));
message.To.Add(new MailboxAddress(string.Empty, "fatul_gb62@itstep.edu.az"));
message.To.Add(new MailboxAddress(string.Empty, "salma_nt63@itstep.edu.az"));
message.From.Add(new MailboxAddress(string.Empty, myAddress));

var imagePart = new MimePart(ContentType.Parse("image/jpeg"));
var htmlPart = new TextPart(TextFormat.Html);

using var image = File.OpenRead("image.jpg");


htmlPart.Text = """
                <ul style="color:#ff00cb;">
                <li>10</li>
                <li>20</li>
                <li>30</li>
                <li>42</li>
                </ul>
                """;

message.Subject = "A picture of a cool car";
message.Body = htmlPart;
message.Sender = sender;

await smtpClient.SendAsync(message);

Console.Read();

// mime types


// application/json
// image/png