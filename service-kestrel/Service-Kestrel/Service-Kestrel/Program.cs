using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_Kestrel.Models;

namespace Service_Kestrel
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<MessagesContext>();
				DataGenerator.Initialize(services);

				var logger = host.Services.GetRequiredService<ILogger<Program>>();
				var options = host.Services.GetRequiredService<ILogger<Program>>();
				var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				logger.LogInformation("Hosting '{1}' is listening on '{0}'", urls, env);
			}

			await host.RunAsync();
		}

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
					webBuilder.UseStartup<Startup>();
				});
	}
}
