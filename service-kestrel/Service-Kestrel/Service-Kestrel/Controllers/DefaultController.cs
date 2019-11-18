using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Kestrel.Dto;
using Service_Kestrel.Models;

namespace Service_Kestrel.Controllers
{
	[Route("api")]
	[ApiController]
    public class DefaultController : ControllerBase
	{
		private readonly MessagesContext messagesContext;

		public DefaultController(MessagesContext messagesContext)
		{
			this.messagesContext = messagesContext;
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
			messagesContext.Messages.Add(new Message { Content = message });
			messagesContext.SaveChanges();
			return Ok();
		}
	}
}
