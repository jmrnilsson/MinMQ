using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
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
		}

		[HttpGet("/status")]
		public IActionResult Status()
		{
			logger.LogInformation("Running Status");
			return Ok(new StatusDto { Text = "ok" });
		}

		[HttpPost("/efcore-in-mem-dto")]
		public async Task PostAsync(MessageDto message)
		{
			logger.LogInformation("Running Post-message");
			await messagesContext.Messages.AddAsync(new Message { Content = message.Content });
			await messagesContext.SaveChangesAsync();
		}

		[HttpPost("/efcore-in-mem-text")]
		public async Task PostAsync(string message)
		{
			await messagesContext.Messages.AddAsync(new Message { Content = message });
			await messagesContext.SaveChangesAsync();
		}
	}
}
