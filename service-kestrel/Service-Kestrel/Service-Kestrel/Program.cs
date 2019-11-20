using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FASTER.core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_Kestrel.Faster;
using Service_Kestrel.Models;

namespace Service_Kestrel
{
	delegate void LogOrPrint(string message, params object[] args);
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// ApplicationLogging.LoggerFactory = new LoggerFactory();
			var host = CreateHostBuilder(args).Build();
			// var logger = ApplicationLogging.LoggerFactory.CreateLogger<Program>();

			using (var scope = host.Services.CreateScope())
			{
				LogOrPrint logOrPrint;
				try
				{
					logOrPrint = new LogOrPrint(host.Services.GetRequiredService<ILogger<Program>>().LogInformation);
				}
				catch
				{
					logOrPrint = new LogOrPrint(Console.WriteLine);
				}

				var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				logOrPrint("Hosted with env '{1}' will listen on '{0}'", urls, env);
			}

			await host.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					// Filter functions could be useful in future:
					// - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.0#filter-functions
					// - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.0&tabs=visual-studio 
					webBuilder.UseLibuv(opt => opt.ThreadCount = 1);
					webBuilder.UseStartup<Startup>();
				}).ConfigureServices(services =>
				{
					services.AddHostedService<FasterCommitHostedService>();
				});
	}
}
