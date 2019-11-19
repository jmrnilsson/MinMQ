using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_Kestrel.Controllers;
using Service_Kestrel.Handlers;
using Service_Kestrel.Models;

namespace Service_Kestrel
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}


		public static IConfiguration Configuration { get; private set; }
		// public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddControllers(configure =>
			{
				configure.Filters.Add<CustomExceptionFilterAttribute>();
			});
			services.AddTransient<CustomExceptionHandler>();
			services.AddDbContext<MessagesContext>(options => options.UseInMemoryDatabase(databaseName: "Messages"));
			services.AddOptions<ServiceKestrelOptions>().Configure(SetOptions).ValidateDataAnnotations();
			services.AddHealthChecks();
			using (CancellationTokenSource source = new CancellationTokenSource())
			{
				var log = FasterContext.Instance.Value.Logger;
				FasterContext.StartCommitInterval(source.Token, log);
			}
		}

		private static void SetOptions(ServiceKestrelOptions o)
		{
			o.FasterDevice = Configuration[nameof(o.FasterDevice)];
		}

		//private static async Task HandleFaster(HttpContext context)
		//{
		//	//app.Run(async context =>
		//	//{
		//	using (StreamReader reader = new StreamReader(context.Request.Body))
		//	{
		//		// var bytes = context.Request.Body.ReadAsByteArrayAsync();
		//		var body = await reader.ReadToEndAsync();
		//		var bytes = Encoding.ASCII.GetBytes(body);
		//		CancellationTokenSource cts = new CancellationTokenSource();
		//		long address = await FasterContext.Instance.Value.Logger.EnqueueAndWaitForCommitAsync(bytes, cts.Token);
		//		//long address = await FasterContext.Instance.Value.Logger.EnqueueAsync(bytes, cts.Token);
		//		//await FasterContext.Instance.Value.Logger.WaitForCommitAsync(address, cts.Token);
		//	}
		//	await context.Response.WriteAsync("{\"ok\": true}");
		//	//});
		//}

		public static void HandleFasterRun(IApplicationBuilder app)
		{
			app.Run(async context =>
			{
				using (StreamReader reader = new StreamReader(context.Request.Body))
				{

					// logger.LogInformation("Running PostFasterAsync - Reading body");
					// var bytes = context.Request.Body.ReadAsByteArrayAsync();
					var body = await reader.ReadToEndAsync();
					var bytes = Encoding.ASCII.GetBytes(body);
					CancellationTokenSource cts = new CancellationTokenSource();
					// logger.LogInformation("Running PostFasterAsync - Enqueue");
					long address = await FasterContext.Instance.Value.Logger.EnqueueAsync(bytes, cts.Token);
					// logger.LogInformation("Running PostFasterAsync - CommitAsync");
					await FasterContext.Instance.Value.Logger.CommitAsync(cts.Token);
					// logger.LogInformation("Running PostFasterAsync - WaitForCommitAsync");
					await FasterContext.Instance.Value.Logger.WaitForCommitAsync(address, cts.Token);
					// long address = await FasterContext.Instance.Value.Logger.EnqueueAndWaitForCommitAsync(bytes, cts.Token);
					// logger.LogInformation("Running PostFasterAsync - Commited");
					//var response = new HttpResponseMessage()
					//{
					//	StatusCode = HttpStatusCode.Created,
					//	Content = new StringContent("ok value!")
					//	//RequestMessage = "ok!"
					// };

					// Response.StatusCode = HttpStatusCode.Created;
					// return await Task.FromResult(response);
					context.Response.StatusCode = 201;
					await context.Response.WriteAsync("Maybe good!?");

				}
				// await Response.WriteAsync("{\"ok\": true, \"handler\", \"yes\"}");
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.Map("/faster", HandleFasterRun);

			new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json")
				.AddEnvironmentVariables()
				.Build();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			//app.UseAuthorization();

			// RequestDelegate fasterDelegate = new RequestDelegate(HandleFaster);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				//endpoints.MapGet("/secret", context =>
				//{
				//	// logger.LogInformation("entry");
				//	context.Response.StatusCode = 200;
				//	// context.Response.WriteAsync("secret");
				//	return Task.CompletedTask;
				//});
				// endpoints.MapGet("/get", HandleFaster);
				//endpoints.MapPost("/faster", HandleFaster);
				endpoints.MapHealthChecks("/healthcheck");
			});
		}
	}

	public class ServiceKestrelOptions
	{
		public string FasterDevice { get; set; }
	}
}
