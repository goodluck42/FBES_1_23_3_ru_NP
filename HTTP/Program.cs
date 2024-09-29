//// Http versions
// HTTP 1.0, 1.1 - TCP/IP
// HTTP 2.0 - TCP/IP
// HTTP 3 - UDP (QUIC)


//// Http methods
// GET - to retrieve data/image/video/file...
// POST - insert/add data/image/video/file...
// PUT - update existing data (with object replacement)
// PATCH - partially update existing data
// DELETE - deletes data


// GET http://localhost/accounts/3?token=myToken&altToken=mytoken123 HTTP/2
// Content-Type: application/json
// Age: 3600
// ------------------------------
// <BODY>

//// Http status codes

// 100 - Info
// 200 - Success
// 300 - Redirection
// 400 - User's errors 
// 500 - Server side errors

using System.Net;

const string pictureUrl = @"https://www.unc.edu/wp-content/uploads/2023/07/snowflakes.1200x675.shutterstock-scaled-1-1200x675.jpg";
using var httpClient = new HttpClient();

httpClient.DefaultRequestVersion = HttpVersion.Version20;

// httpClient.DefaultRequestHeaders.Add("X-MyHeader", "MyValue");

try
{
	// image/jpeg
	var httpResponseMessage = (await httpClient.GetAsync(pictureUrl)).EnsureSuccessStatusCode();
	var contentType = httpResponseMessage.Content.Headers.ContentType?.MediaType;
	
	if (contentType is "image/jpeg")
	{
		using var fileStream = File.OpenWrite($"{Guid.NewGuid()}.jpeg");
		
		await httpResponseMessage.Content.CopyToAsync(fileStream);
	}
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
}



