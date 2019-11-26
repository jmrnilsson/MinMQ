using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MinMQ.Service.HttpRequestHandlers
{
	public static class FasterHttpHandler
	{
		public static async Task HandleRequest(HttpContext context)
		{
			using (StreamReader reader = new StreamReader(context.Request.Body))
			{
				var body = await reader.ReadToEndAsync();
				var bytes = Encoding.ASCII.GetBytes(body);
				CancellationTokenSource cts = new CancellationTokenSource();
				long address = await FasterOps.Instance.Value.EnqueueAsync(bytes, cts.Token);
				// await FasterWriter.Instance.Value.CommitAsync(cts.Token);
				await FasterOps.Instance.Value.WaitForCommitAsync(address, cts.Token);
				context.Response.StatusCode = 201;
				await context.Response.WriteAsync("Created");
			}
		}
	}
}
