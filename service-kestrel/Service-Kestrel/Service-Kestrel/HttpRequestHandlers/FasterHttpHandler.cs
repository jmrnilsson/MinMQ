using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MinMQ.Service.HttpRequestHandlers
{
	public static class FasterHttpHandler
	{
		private static SemaphoreSlim flush = new SemaphoreSlim(8, 8);

		public static async Task HandleRequest(HttpContext context)
		{
			await flush.WaitAsync();

			try
			{
				using (StreamReader reader = new StreamReader(context.Request.Body))
				{
					var body = await reader.ReadToEndAsync();
					var bytes = Encoding.ASCII.GetBytes(body);
					CancellationTokenSource cts = new CancellationTokenSource();
					long address = await FasterOps.Instance.Value.EnqueueAsync(bytes, cts.Token);
					await FasterOps.Instance.Value.CommitAsync(cts.Token);
					await FasterOps.Instance.Value.WaitForCommitAsync(address, cts.Token);
					context.Response.StatusCode = 201;
					await context.Response.WriteAsync("Created");
				}
			}
			catch (Exception)
			{
				flush.Release();
			}
		}
	}
}
