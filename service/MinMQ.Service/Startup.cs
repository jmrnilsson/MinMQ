using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinMQ.Service.Configuration;
using MinMQ.Service.Filters;
using MinMQ.Service.HttpRequestHandlers;
using MinMq.Service.Models;
using MinMQ.Service.Models;
using MinMq.Service.Repository;

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
			services.AddDbContext<MessageQueueContext>(options => options.UseNpgsql(Configuration.GetConnectionString("MessageQueueContext")));
			services.AddScoped<IMessageRepository, MessageRepository>();
			services.AddScoped<IQueueRepository, QueueRepository>();
			services.AddScoped<IMimeTypeRepository, MimeTypeRepository>();
			services.AddScoped<ICursorRepository, CursorRepository>();
		}

		private static void SetOptions(MinMQConfiguration o)
		{
			o.FasterDevice = Configuration[nameof(o.FasterDevice)];
			o.ScanFlushSize = int.Parse(Configuration[nameof(o.ScanFlushSize)]);
			o.ConnectionStringPostgres = Configuration.GetSection("ConnectionStrings")["MessageQueueContext"];
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			Console.WriteLine("Environment setting {0}", env.EnvironmentName);

			new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json")
				.AddEnvironmentVariables()
				.Build();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			UpdateDatabase(app);

			app.UseRouting();

			// app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapPost("/send", FasterHttpHandler.HandleRequest);
				endpoints.MapHealthChecks("/healthcheck");
			});
		}

		private static void UpdateDatabase(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				using (var context = serviceScope.ServiceProvider.GetService<MessageQueueContext>())
				{
					context.Database.Migrate();
				}
			}
		}
	}
}
