using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinMq.Service.RequestHandlers
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
				long address = await FasterWriter.Instance.Value.EnqueueAsync(bytes, cts.Token);
				// await FasterWriter.Instance.Value.CommitAsync(cts.Token);
				await FasterWriter.Instance.Value.WaitForCommitAsync(address, cts.Token);
				context.Response.StatusCode = 201;
				await context.Response.WriteAsync("Created");
			}
		}
	}
}
