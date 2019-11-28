using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinMq.Service.Models;
using MinMQ.Service.Configuration;
using MinMQ.Service.Filters;
using MinMQ.Service.HttpRequestHandlers;
using MinMQ.Service.Models;

namespace MinMQ.Service
{
	public delegate CancellationTokenSource CancellationTokenSourceFactory();

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public static IConfiguration Configuration { get; private set; }
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddControllers(configure =>
			{
				configure.Filters.Add<CustomExceptionFilterAttribute>();
			});
			services.AddTransient<CustomExceptionHandler>();
			services.AddDbContext<MessagesContext>(options => options.UseInMemoryDatabase(databaseName: "Messages"));
			services.AddOptions<MinMQConfiguration>().Configure(SetOptions).ValidateDataAnnotations();
			services.AddHealthChecks();
			services.AddScoped<CancellationTokenSourceFactory>(_ => () => new CancellationTokenSource());
			// services.AddDbContext<MessageQueueContext>(options => options.UseNpgsql(Configuration.GetConnectionString("MessageQueueContext")));
			services.AddDbContext<MessageQueueContext>(options => options.UseNpgsql(Configuration.GetConnectionString("MessageQueueContext")));
		}

		private static void SetOptions(MinMQConfiguration o)
		{
			o.FasterDevice = Configuration[nameof(o.FasterDevice)];
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
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

			// app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapPost("/send", FasterHttpHandler.HandleRequest);
				endpoints.MapHealthChecks("/healthcheck");
			});
		}
	}
}
