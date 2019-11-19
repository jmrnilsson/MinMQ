using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service_Kestrel.Dto;
using Service_Kestrel.Models;

namespace Service_Kestrel.Controllers
{
	[Route("api")]
	[ApiController]
	public class DefaultController : ControllerBase
	{
		private readonly MessagesContext messagesContext;
		private readonly ILogger<DefaultController> logger;

		public DefaultController(MessagesContext messagesContext, ILogger<DefaultController> logger)
		{
			this.messagesContext = messagesContext;
			this.logger = logger;
			logger.LogInformation("default controller some info");
		}

		[HttpGet("/status")]
		public IActionResult Status()
		{
			logger.LogInformation("Running Status");
			return Ok(new StatusDto { Text = "ok" });
		}

		[HttpPost("/message")]
		public async Task PostAsync(MessageDto message)
		{
			logger.LogInformation("Running Post-message");
			await messagesContext.Messages.AddAsync(new Message { Content = message.Content });
			await messagesContext.SaveChangesAsync();
		}

		[HttpPost("/message-text")]
		public async Task PostAsync(string message)
		{
			await messagesContext.Messages.AddAsync(new Message { Content = message });
			await messagesContext.SaveChangesAsync();
		}

		[HttpPost("/faster-text")]
		public async Task<HttpResponseMessage> PostFasterAsync(string message)
		{
			// logger.LogInformation("Running PostFasterAsync");

			using (StreamReader reader = new StreamReader(Request.Body))
			{

				// logger.LogInformation("Running PostFasterAsync - Reading body");
				// var bytes = context.Request.Body.ReadAsByteArrayAsync();
				var body = await reader.ReadToEndAsync();
				var bytes = Encoding.ASCII.GetBytes(body);
				CancellationTokenSource cts = new CancellationTokenSource();
				// logger.LogInformation("Running PostFasterAsync - Enqueue");
				long address = await FasterContext.Instance.Value.Logger.EnqueueAsync(bytes, cts.Token);
				// logger.LogInformation("Running PostFasterAsync - CommitAsync");
				await FasterContext.Instance.Value.Logger.CommitAsync(cts.Token);
				// logger.LogInformation("Running PostFasterAsync - WaitForCommitAsync");
				await FasterContext.Instance.Value.Logger.WaitForCommitAsync(address, cts.Token);
				// long address = await FasterContext.Instance.Value.Logger.EnqueueAndWaitForCommitAsync(bytes, cts.Token);
				// logger.LogInformation("Running PostFasterAsync - Commited");
				var response = new HttpResponseMessage()
				{
					StatusCode = HttpStatusCode.Created,
					Content = new StringContent("ok value!")
					//RequestMessage = "ok!"
				};
				return await Task.FromResult(response);
			}
			// await Response.WriteAsync("{\"ok\": true, \"handler\", \"yes\"}");
		}
	}
}
