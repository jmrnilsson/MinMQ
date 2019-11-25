using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MinMq.Service.Filters
{
	public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private readonly CustomExceptionHandler handler;

		public CustomExceptionFilterAttribute(CustomExceptionHandler handler)
		{
			this.handler = handler;
		}

		public override void OnException(ExceptionContext context)
		{
			handler.ActOn(context.Exception);
		}

		public async override Task OnExceptionAsync(ExceptionContext context)
		{
			OnException(context);
			await Task.CompletedTask;
		}
	}

	public class CustomExceptionHandler
	{
		private readonly ILogger<CustomExceptionHandler> logger;

		public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
		{
			this.logger = logger;
		}

		public void ActOn(Exception exception)
		{
			if (exception is BadHttpRequestException)
			{
				logger.LogInformation("A suspected thread starvation exceptions occured.");
				return;
			}
			logger.LogError("Unhandled error={0}", exception);
		}
	}
}
