using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FASTER.core;
using MinMQ.Service.Configuration;
using Optional;

namespace MinMQ.Service
{
	public sealed class FasterOps : IFasterWriter
	{
		private readonly IDevice device;
		private readonly FasterLog logger;
		private long nextAddress = 0;

		private FasterOps()
		{
			string devicePath = Startup.Configuration[nameof(MinMQConfiguration.FasterDevice)];
			device = Devices.CreateLogDevice(devicePath);
			logger = new FasterLog(new FasterLogSettings { LogDevice = device });
		}

		public static Lazy<FasterOps> Instance { get; set; } = new Lazy<FasterOps>(() => new FasterOps());

		#region Write
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
		#endregion

		#region Read
		public async Task<Option<(string, long, long)>> GetNext()
		{
			using (FasterLogScanIterator iter = logger.Scan(nextAddress, 100_000_000))
			{
				while (true)
				{
					byte[] entry;
					int entryLenght;

					while (!iter.GetNext(out entry, out entryLenght))
					{
						if (iter.CurrentAddress >= 100_000_000) return Option.None<(string, long, long)>();
					}

					ASCIIEncoding ascii = new ASCIIEncoding();
					await iter.WaitAsync();
					nextAddress = iter.NextAddress;
					return Option.Some((ascii.GetString(entry), iter.CurrentAddress, iter.NextAddress));  // Possible to pipe
				}
			}
		}

		public async Task<List<(string, long, long)>> GetList()
		{
			var result = new List<(string, long, long)>();
			using (FasterLogScanIterator iter = logger.Scan(nextAddress, 100_000_000))
			{
				int i = 0;
				byte[] entry;
				int entryLenght;
				while (iter.GetNext(out entry, out entryLenght))
				{
					ASCIIEncoding ascii = new ASCIIEncoding();
					if (iter.CurrentAddress >= 1568) Debugger.Break();
					await iter.WaitAsync();
					result.Add((ascii.GetString(entry), iter.CurrentAddress, iter.NextAddress));
					i++;
					if (i > 50)
					{
						nextAddress = iter.NextAddress;
						break;
					}
				}
			}
			return result;
		}

		public async Task<List<(string, long, long)>> GetListAsync()
		{
			// Examples:
			// - https://github.com/microsoft/FASTER/blob/ff51d3c973634d043df08cd79e6ba5a0e1ffa6c1/cs/playground/FasterLogSample/Program.cs
			// Implementation of awaiter:
			// - https://github.com/microsoft/FASTER/blob/98889aeea31041aa03c056c61abfd3e559d53a21/cs/src/core/Index/FasterLog/FasterLogIterator.cs#L144
			var result = new List<(string, long, long)>();

			// using (FasterLogScanIterator iter = logger.Scan(logger.BeginAddress, 100_000_000, name: nameof(GetListAsync)))
			using (FasterLogScanIterator iter = logger.Scan(nextAddress, 100_000_000))
			{
				int i = 0;
				await foreach ((byte[] bytes, int length) in iter.GetAsyncEnumerable())
				{
					ASCIIEncoding ascii = new ASCIIEncoding();
					// if (iter.CurrentAddress >= 1568) Debugger.Break();
					await iter.WaitAsync();
					result.Add((ascii.GetString(bytes), iter.CurrentAddress, iter.NextAddress));
					i++;
					if (i > 50)
					{
						nextAddress = iter.NextAddress;
						break;
					}
				}
			}

			return result;
		}
		#endregion
	}
}
