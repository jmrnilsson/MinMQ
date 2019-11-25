using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinMq.Service.Faster;

namespace MinMq.Service
{
	delegate void LogOrPrint(string message, params object[] args);

	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
	
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
				logOrPrint("Starting service. Env='{0}' Urls='{1}'", env, urls);
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
					webBuilder.UseLibuv();
					webBuilder.UseStartup<Startup>();
				}).ConfigureServices(services =>
				{
					services.AddHostedService<FasterCommitHostedService>();
				});
	}
}
