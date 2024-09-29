using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;

var myMail = "xrlogic1@gmail.com";
using var fileStream =
	File.OpenRead("client_secret_62592329031-akl3jt1co1k5gt5rhinrhcp2qvqus55j.apps.googleusercontent.com.json");

var googleClientSecrets = await GoogleClientSecrets.FromStreamAsync(fileStream);
var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
{
	ClientSecrets = googleClientSecrets.Secrets,
	Scopes = [GmailService.Scope.GmailReadonly]
});

var codeReceiver = new LocalServerCodeReceiver();
var app = new AuthorizationCodeInstalledApp(flow, codeReceiver);
var credentials = await app.AuthorizeAsync(myMail, CancellationToken.None);

if (app.ShouldRequestAuthorizationCode(credentials.Token))
{
	await credentials.RefreshTokenAsync(CancellationToken.None);
}

// var saslMechanism = new SaslMechanismOAuth2(credentials.UserId, credentials.Token.AccessToken);
// using var imapClient = new ImapClient { };
// await imapClient.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
// await imapClient.AuthenticateAsync(saslMechanism);
// await imapClient.DisconnectAsync(true);

using var gmailService = new GmailService(new BaseClientService.Initializer
{
	HttpClientInitializer = credentials,
	ApplicationName = "MailApp",
});


var labelsRequest = gmailService.Users.Labels.List("me");
var labelsResponse = await labelsRequest.ExecuteAsync();
var inboxLabel = labelsResponse.Labels.Single(l => l.Name == "INBOX");
var request = gmailService.Users.Messages.List("me");
var messagesResponse = await request.ExecuteAsync();

Console.WriteLine(inboxLabel.MessagesTotal);

if (messagesResponse.Messages is
    {
	    Count: 0
    })
{
	Console.WriteLine("No messages.");
}
else
{
	foreach (var message in messagesResponse.Messages.Take(3))
	{
		Console.WriteLine("----------------------");
		var messageWrapper = await gmailService.Users.Messages.Get("me", message.Id).ExecuteAsync();

		if (messageWrapper is not null)
		{
			// HTTP Headers info
			foreach (var header in messageWrapper.Payload.Headers)
			{
				Console.WriteLine($"header: {header.Name} = {header.Value}");
			}

			// var from = string.Empty;
			// var date = string.Empty;
			// var subject = string.Empty;
			// var body = string.Empty;
			//
			// foreach (var mParts in messageWrapper.Payload.Headers)
			// {
			// 	if (mParts.Name == "Date")
			// 	{
			// 		date = mParts.Value;
			// 	}
			// 	else if (mParts.Name == "From")
			// 	{
			// 		from = mParts.Value;
			// 	}
			// 	else if (mParts.Name == "Subject")
			// 	{
			// 		subject = mParts.Value;
			// 	}
			//
			// 	if (date != string.Empty && from != string.Empty)
			// 	{
			// 		if (messageWrapper.Payload.Parts == null && messageWrapper.Payload.Body != null)
			// 		{
			// 			body = messageWrapper.Payload.Body.Data;
			// 		}
			// 		// else
			// 		{
			// 			body = GetNestedParts(messageWrapper.Payload.Parts, "");
			// 		}
			//
			// 		Console.WriteLine($"body: {body}");
			// 	}
			//
			// 	//Console.WriteLine($"body: {Encoding.UTF8.GetString(Convert.FromBase64String(body))}");
			// 	;
			// }
			//
			// //Console.Write(body);
			// Console.WriteLine("{0}  --  {1}  -- {2}", subject, date, messageWrapper.Id);
		}
		
	}
}

String GetNestedParts(IList<MessagePart>? part, string curr)
{
	string str = curr;
	if (part == null)
	{
		return str;
	}
	else
	{
		foreach (var parts in part)
		{
			if (parts.Parts  == null)
			{
				if (parts.Body != null && parts.Body.Data != null)
				{
					str += parts.Body.Data;
				}
			}
			else
			{
				return GetNestedParts(parts.Parts, str);
			}
		}

		return str;
	}        
}