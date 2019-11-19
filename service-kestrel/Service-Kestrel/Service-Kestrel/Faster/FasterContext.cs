using FASTER.core;
using Microsoft.Extensions.Logging;
using Service_Kestrel.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service_Kestrel
{
	public sealed class FasterContext
	{
		private ILogger logger;
		public IDevice Device { get; }
        public FasterLog Logger { get; }

        public static Lazy<FasterContext> Instance = new Lazy<FasterContext>(() => new FasterContext());

		private FasterContext()
		{
			try
			{
				logger = new LoggerFactory().CreateLogger<FasterContext>();
				// string deviceFormattedPath;
				string devicePath = Startup.Configuration[nameof(ServiceKestrelOptions.FasterDevice)];

				//if (devicePath.Contains("opt/"))
				//{
				//	deviceFormattedPath = $"{Path.GetTempPath()}hlog.log";
				//}
				//else
				//{
				logger.LogInformation("DevicePath+GetFullPath={0}, OldDevicePath+GetFullPath={1, TempPath={2}", Path.GetFullPath(devicePath), Path.GetFullPath("/opt/faster/hlog.log"), Path.GetTempPath());
				// }

				Device = Devices.CreateLogDevice(devicePath);
				Logger = new FasterLog(new FasterLogSettings { LogDevice = Device });
			}
			catch (Exception error)
			{
				logger.LogError("Error starting FasterLog. Error={0}", error);
			}
		}

		public static void StartCommitInterval(CancellationToken token, FasterLog log)
		{
			var state = new FasterCommitState(log, token);
			var loop = new FasterCommitCallback();
			var stateTimer = new Timer(loop.Execute, state, 0, 30);
		}
	}
}
