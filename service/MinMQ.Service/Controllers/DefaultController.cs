using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MinMQ.Service.Controllers.Dto;
using MinMq.Service.Entities;
using MinMQ.Service.Models;
using MinMq.Service.Repository;

namespace MinMQ.Service.Controllers
{
	[Route("api")]
	[ApiController]
	public class DefaultController : ControllerBase
	{
		private readonly MessagesContext messagesContext;
		private readonly IQueueRepository queueRepository;

		public DefaultController(MessagesContext messagesContext, IQueueRepository queueRepository)
		{
			this.messagesContext = messagesContext;
			this.queueRepository = queueRepository;
		}

		[HttpGet("/status")]
		public IActionResult Status()
		{
			return Ok(new StatusResponseDto { Text = "ok" });
		}

		[HttpPost("/efcore-in-mem-dto")]
		public async Task PostAsync(MessageRequestDto message)
		{
			await messagesContext.Messages.AddAsync(new Models.Message { Content = message.Content });
			await messagesContext.SaveChangesAsync();
		}

		[HttpPost("/efcore-in-mem-text")]
		public async Task PostAsync(string message)
		{
			await messagesContext.Messages.AddAsync(new Models.Message { Content = message });
			await messagesContext.SaveChangesAsync();
		}

		[HttpGet("/peek")]
		public async Task<IActionResult> Peek()
		{
			var option = await FasterOps.Instance.Value.GetNext();

			return option.Match<IActionResult>
			(
				  some: value => Ok(value.ToPeekResponse()),
				  none: () => NotFound()
			);
		}

		[HttpGet("/list")]
		public async Task<IActionResult> List()
		{
			var scanner = FasterOps.Instance.Value.GetListAsync();
			return (await ControllerExtensions.ToListResponse(scanner)).Match<IActionResult>
			(
				some: values => Ok(values),
				none: () => NotFound()
			);
		}

		// In contrast, the URI in a PUT request identifies the entity enclosed with the request.
		[HttpPut("/queue/{name:regex(^\\w+)}")]
		public async Task<IActionResult> List(string name)
		{
			return Ok(await queueRepository.Add(new Queue(name)));
		}
	}
}
