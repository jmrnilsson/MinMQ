using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service_Kestrel.RequestHandlers
{
	public static class FasterHttpHandler
	{
		// private static SemaphoreSlim requestThrottling { get; set; } = new SemaphoreSlim(3, 3);

		public static async Task HandleRequest(HttpContext context)
		{
			await requestThrottling.WaitAsync();

			try
			{

			using (StreamReader reader = new StreamReader(context.Request.Body))
			{
				var body = await reader.ReadToEndAsync();
				var bytes = Encoding.ASCII.GetBytes(body);
				CancellationTokenSource cts = new CancellationTokenSource();
				long address = await FasterWriter.Instance.Value.EnqueueAsync(bytes, cts.Token);
				await FasterWriter.Instance.Value.CommitAsync(cts.Token);
				await FasterWriter.Instance.Value.WaitForCommitAsync(address, cts.Token);
				context.Response.StatusCode = 201;
				await context.Response.WriteAsync("Created");
			}
			}
			catch (Exception e) when (e.Message.Contains("Unable to add item to list"))
			{
				context.Response.StatusCode = 503;
				await context.Response.WriteAsync("Busy");
			}

			requestThrottling.Release();

		}
	}
}
