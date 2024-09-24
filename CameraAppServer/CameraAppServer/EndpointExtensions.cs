using System.Net;

namespace CameraAppServer;

public static class EndpointExtensions
{
	public static IPAddress? GetIPAddress(this EndPoint? endPoint)
	{
		return endPoint.AsIPEndpoint()?.Address;
	}

	public static IPEndPoint? AsIPEndpoint(this EndPoint? endPoint)
	{
		return endPoint as IPEndPoint;
	}
}