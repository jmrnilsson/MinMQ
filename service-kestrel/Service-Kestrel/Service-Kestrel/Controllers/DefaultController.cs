using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			return Ok(new StatusDto { Text = "ok" });
		}

		[HttpPost("/message")]
		public async Task PostAsync(MessageDto message)
		{
			await messagesContext.Messages.AddAsync(new Message { Content = message.Content });
			await messagesContext.SaveChangesAsync();
		}

		[HttpPost("/message-text")]
		public async Task PostAsync(string message)
		{
			await messagesContext.Messages.AddAsync(new Message { Content = message });
			await messagesContext.SaveChangesAsync();
		}

		[HttpPost("/message-number")]
		public async Task PostAsync(int number)
		{
			await messagesContext.Messages.AddAsync(new Message { Content = Convert.ToString(number) }); ;
			await messagesContext.SaveChangesAsync();
		}

		[HttpPost("/message-text-sync")]
		public IActionResult Post(string message)
		{
			logger.LogInformation("text-sync. %s", message);
			messagesContext.Messages.Add(new Message { Content = message });
			messagesContext.SaveChanges();
			return Ok();
		}

		[HttpPost("/x")]
		public async Task X()
		{
			await Task.CompletedTask;
		}

		[HttpGet("/message-faster")]
		public async Task PostFaster()
		{
			using (StreamReader reader = new StreamReader(Request.Body))
			{
				// var bytes = context.Request.Body.ReadAsByteArrayAsync();
				var body = await reader.ReadToEndAsync();
				var bytes = Encoding.ASCII.GetBytes(body);
				CancellationTokenSource cts = new CancellationTokenSource();
				long address = await FasterContext.Instance.Value.Logger.EnqueueAsync(bytes, cts.Token);
				await FasterContext.Instance.Value.Logger.WaitForCommitAsync(address, cts.Token);
			}
			await Response.WriteAsync("{\"ok\": true}");
		}
	}
}
