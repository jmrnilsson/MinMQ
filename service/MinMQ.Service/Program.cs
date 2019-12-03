using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinMQ.Service.Faster;

namespace MinMQ.Service
{
	internal delegate void LogOrPrint(string message, params object[] args);

	public class Program
	{
		private static HashSet<string> stoppingProcesses = new HashSet<string>();
		// private static IHost host;
		// private static Task started;

		public static async Task Main(string[] args)
		{
			// started = Task.Delay(10_000);
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
			// started = Task.CompletedTask;
		}

		public static void Close(string nameOfStoppingProcess)
		{
			Console.WriteLine("Program recieved stop notication from {0}", nameOfStoppingProcess);
			stoppingProcesses.Add(nameOfStoppingProcess);

			if (stoppingProcesses.Contains(nameof(FasterHostedServiceCommit)) && stoppingProcesses.Contains(nameof(FasterHostedServiceMoveData)))
			{
				Console.WriteLine("Stopping program");

				// throw new ApplicationException("Stopping application");

				// await started;
				// await host.StopAsync();
				// Environment.Exit(1);
			}
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
					services.AddHostedService<FasterHostedServiceCommit>();
					services.AddHostedService<FasterHostedServiceMoveData>();
				});
	}
}
