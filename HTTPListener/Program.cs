using System.Net;

var manualResetEvent = new ManualResetEventSlim(false);
var httpListener = new HttpListener();

// MyNetwork
// Qwerty123
// netsh http add urlacl url=http://10.0.0.200:12345/ user=Alex

httpListener.Prefixes.Add("http://10.0.0.200:12345/");
httpListener.Start();

_ = Task.Run(async () =>
{
	try
	{
		using (httpListener)
		{
			while (true)
			{
				var context = await httpListener.GetContextAsync();
				var path = GetUrlPath(context.Request.Url?.ToString());
				await using var streamWriter = new StreamWriter(context.Response.OutputStream);

				switch (path)
				{
					case "index.html":
					{
						await using var fileStream = File.OpenRead("Pages/index.html");
						await fileStream.CopyToAsync(streamWriter.BaseStream);

						break;
					}
					default:
						context.Response.StatusCode = (int)HttpStatusCode.NotFound;
						break;
				}
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
});

manualResetEvent.Wait();

return;

static string GetUrlPath(string? pathIn)
{
	if (pathIn is null)
	{
		return string.Empty;
	}
	
	try
	{
		return string.Join('/', pathIn.Split("//").Last().Split('/').Skip(1));
	}
	catch
	{
		return string.Empty;
	}
}