using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;

namespace Service_Kestrel.Exceptions
{
	public class SuspectedThreadStarvationException : Exception
	{
		public SuspectedThreadStarvationException(BadHttpRequestException exception) : base(exception.Message, exception)
		{
		}
	}
}
