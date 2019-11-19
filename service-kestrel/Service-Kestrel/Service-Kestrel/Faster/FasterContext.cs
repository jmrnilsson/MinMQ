using FASTER.core;
using Service_Kestrel.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service_Kestrel
{
	public sealed class FasterContext
	{
		private readonly IDevice device;
		private readonly FasterLog log;
		public IDevice Device => device;
		public FasterLog Logger => log;

		public static Lazy<FasterContext> Instance = new Lazy<FasterContext>(() => new FasterContext());

		private FasterContext()
		{
			try
			{
				// Static variant
				string devicePath = Startup.Configuration[nameof(ServiceKestrelOptions.FasterDevice)];
				device = Devices.CreateLogDevice(devicePath);
				log = new FasterLog(new FasterLogSettings { LogDevice = device });
			}
			catch (Exception error)
			{
				Console.WriteLine("Error starting FasterLog. Error=" + error.ToString());
			}
		}

		public static void StartCommitInterval(CancellationToken token, FasterLog log)
		{
			// Create an AutoResetEvent to signal the timeout threshold in the
			// timer callback has been reached.
			var state = new CommitState(log, token);
			var loop = new CommitLoop();

			var stateTimer = new Timer(loop.Execute, state, 0, 30);

			//// When autoEvent signals, change the period to every half second.
			//autoEvent.WaitOne();
			// stateTimer.Change(0, 500);

			// When autoEvent signals the second time, dispose of the timer.
			//autoEvent.WaitOne();
			//stateTimer.Dispose();
		}
	}
}
