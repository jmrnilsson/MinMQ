using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service_Kestrel.Controllers
{
    [Route("api")]
    [ApiController]
    public class StatusController : ControllerBase
	{
		[HttpGet("/status")]
		public async Task<StatusDto> Status()
		{
			return await Task.FromResult(new StatusDto { Text = "ok" });
		}
	}
}
