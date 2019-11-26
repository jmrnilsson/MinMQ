using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MinMQ.BenchmarkConsole
{
	public class HttpCustomHeaderHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync
		(
			HttpRequestMessage request,
			CancellationToken cancellationToken
		)
		{
			return base.SendAsync(request, cancellationToken).ContinueWith(
				(task) =>
				{
					HttpResponseMessage response = task.Result;
					response.Headers.Add("X-Custom-Header", "Some value");
					return response;
				}
			);
		}
	}
}
