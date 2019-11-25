using System;
using System.Threading;
using System.Threading.Tasks;
using FASTER.core;
using MinMQ.Service.Configuration;

namespace MinMQ.Service
{
	public sealed class FasterWriter : IFasterWriter
	{
		private readonly IDevice device;
		private readonly FasterLog logger;

		private FasterWriter()
		{
			string devicePath = Startup.Configuration[nameof(MinMQConfiguration.FasterDevice)];
			device = Devices.CreateLogDevice(devicePath);
			logger = new FasterLog(new FasterLogSettings { LogDevice = device });
		}

		public static Lazy<FasterWriter> Instance { get; set; } = new Lazy<FasterWriter>(() => new FasterWriter());

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
