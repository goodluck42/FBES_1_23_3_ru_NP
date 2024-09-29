using System.Net;

var manualResetEvent = new ManualResetEventSlim(false);
var httpListener = new HttpListener();


// netsh http add urlacl url=http://10.0.0.200:12345/ user=Alex

httpListener.Prefixes.Add("http://10.0.0.200:12345/");

httpListener.Start();


_ = Task.Run(async () =>
{
	using (httpListener)
	{
		while (true)
		{
			var context = await httpListener.GetContextAsync();
			using var streamWriter = new StreamWriter(context.Response.OutputStream);
			
			streamWriter.Write(@"<h1 style=""color:red;"">Hello world</h1>");
		}
	}
});


manualResetEvent.Wait();