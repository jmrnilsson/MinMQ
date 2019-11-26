using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MinMQ.Service.Controllers.Dto;
using MinMQ.Service.Models;
using Optional.Unsafe;

namespace MinMQ.Service.Controllers
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
			return Ok(new StatusResponseDto { Text = "ok" });
		}

		[HttpPost("/efcore-in-mem-dto")]
		public async Task PostAsync(MessageRequestDto message)
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

		[HttpGet("/peek")]
		public async Task<IActionResult> Peek()
		{
			var option = await FasterOps.Instance.Value.GetNext();

			IActionResult response = null;

			option.Match
			(
				  some: value => response = Ok(value.ToPeekResponse()),
				  none: () => response = NotFound()
			);

			return response;
		}

		[HttpGet("/list")]
		public async Task<IActionResult> List()
		{
			var result = await FasterOps.Instance.Value.GetListAsync();
			return Ok(result.ToListResponse());
		}
	}
}
