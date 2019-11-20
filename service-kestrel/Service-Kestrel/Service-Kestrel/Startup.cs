using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service_Kestrel.Configuration;
using Service_Kestrel.Filters;
using Service_Kestrel.Models;
using Service_Kestrel.RequestHandlers;

namespace Service_Kestrel
{
	public delegate CancellationTokenSource CancellationTokenSourceFactory();
	
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
			services.AddOptions<ServiceKestrelConfiguration>().Configure(SetOptions).ValidateDataAnnotations();
			services.AddHealthChecks();
			services.AddScoped<CancellationTokenSourceFactory>(_ => () => new CancellationTokenSource());
		}

		private static void SetOptions(ServiceKestrelConfiguration o)
		{
			o.FasterDevice = Configuration[nameof(o.FasterDevice)];
			o.LogCommitPollingEverySeconds = int.Parse(Configuration[nameof(o.LogCommitPollingEverySeconds)]);
		}

		// public static void HandleFasterRun(IApplicationBuilder app)


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.Map("/faster", a => a.Run(FasterHttpHandler.HandleRequest));

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

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapPost("/faster-get", FasterHttpHandler.HandleRequest);
				endpoints.MapHealthChecks("/healthcheck");
			});
		}
	}
}
