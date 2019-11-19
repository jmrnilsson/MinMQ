using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Kestrel
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
			handler.ReportError(context.Exception);
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

		public void ReportError(Exception exception)
		{
			logger.LogError("Unhandled error={0}", exception);
		}
	}
}
