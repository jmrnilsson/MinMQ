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
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// ApplicationLogging.LoggerFactory = new LoggerFactory();
			var host = CreateHostBuilder(args).Build();
			// var logger = ApplicationLogging.LoggerFactory.CreateLogger<Program>();

			using (var scope = host.Services.CreateScope())
			{
				ILogger<Program> logger = null;

				try
				{
					logger = host.Services.GetRequiredService<ILogger<Program>>();
				}
				catch (Exception error)
				{
					Console.WriteLine("Could not start logger. Error=" + error);
				}

				//var services = scope.ServiceProvider;
				//DataGenerator.Initialize(services);
				var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				logger.LogInformation("Hosted with env '{1}' will listen on '{0}'", urls, env);
				// var cts = host.Services.GetRequiredService<CancellationTokenSource>();

				//using (var cts = new CancellationTokenSource())
				//{
				//	logger.LogInformation("Starting polling..");
				//	var t = new Thread(CreateCommitThread(logger, FasterContext.Instance.Value.Logger));
				//	t.Start();
				//	t.Join();
				//	//FasterContext.StartCommitInterval(cts.Token, FasterContext.Instance.Value.Logger, logger);
				//}

			}

			await host.RunAsync();
		}

		//static ThreadStart CreateCommitThread(ILogger logger, FasterLog log)
		//{
		//	return new ThreadStart(() =>
		//	{
		//		int invokationCount = 0;
		//		while (true)
		//		{

		//			Thread.Sleep(50);
		//			log.Commit(true);

		//			if (invokationCount > 0 && invokationCount % 99 == 0)
		//			{
		//				logger.LogInformation("Polling % 99 == 0 executed..");
		//				invokationCount = 0;
		//			}
		//			else
		//			{
		//				invokationCount++;
		//			}
		//		}
		//	});
		//}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.0
					//webBuilder.ConfigureLogging(logBuilder =>
					// {
					//	 logBuilder.AddFilter((provider, category, logLevel) =>
					//	 {
					//		//if (provider == "Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider" &&
					//		 if (category.StartsWith("Service_Kestrel") && logLevel >= LogLevel.Information)
					//		 {
					//			 return true;
					//		 }
					//		 return false;
					//	 });
					// });
					webBuilder.UseLibuv(opt => opt.ThreadCount = 1);
					webBuilder.UseStartup<Startup>();
					// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.0&tabs=visual-studio
				}).ConfigureServices(services =>
				{
					services.AddHostedService<FasterCommitHostedService>();
				});
	}
}
