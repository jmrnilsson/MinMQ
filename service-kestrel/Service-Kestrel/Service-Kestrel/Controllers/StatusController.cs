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
		[HttpGet("/status-async")]
		public async Task<StatusDto> StatusAsync()
		{
			return await Task.FromResult(new StatusDto { Text = "ok" });
		}

		[HttpGet("/status")]
		public IActionResult Status()
		{
			return Ok(new StatusDto { Text = "ok" });
		}

	}
}
