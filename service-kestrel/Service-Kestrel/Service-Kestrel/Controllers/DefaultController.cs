using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MinMq.Service.Controllers.Dto;
using MinMq.Service.Models;

namespace MinMq.Service.Controllers
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

		[HttpPost("/efcore-in-mem-dto")]
		public async Task PostAsync(MessageDto message)
		{
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
