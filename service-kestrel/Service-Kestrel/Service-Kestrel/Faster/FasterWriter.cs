using FASTER.core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service_Kestrel
{
	public sealed class FasterWriter : IFasterWriter
	{
		private IDevice device;
		private FasterLog logger;

		public static Lazy<FasterWriter> Instance = new Lazy<FasterWriter>(() => new FasterWriter());

		private FasterWriter()
		{
			string devicePath = Startup.Configuration[nameof(ServiceKestrelOptions.FasterDevice)];
			device = Devices.CreateLogDevice(devicePath);
			logger = new FasterLog(new FasterLogSettings { LogDevice = device });
		}

		public async ValueTask CommitAsync(CancellationToken token = default)
		{
			await logger.CommitAsync(token);
		}

		public async ValueTask WaitForCommitAsync(long untilAddress = 0, CancellationToken token = default)
		{
			await logger.WaitForCommitAsync(untilAddress, token);
		}

		public async ValueTask<long> EnqueueAsync(byte[] entry, CancellationToken token = default)
		{
			return await logger.EnqueueAsync(entry, token);
		}
	}
}
