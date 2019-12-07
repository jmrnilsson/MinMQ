using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FASTER.core;
using MinMq.Service.Entities;
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
			logger = new FasterLog(new FasterLogSettings { LogDevice = device, });
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

		public void TruncateUntil(long address)
		{
			logger.TruncateUntil(address);
		}
		#endregion

		#region Read
		public async Task<Option<(string, long, long)>> GetNext()
		{
			using FasterLogScanIterator iter = logger.Scan(nextAddress, 100_000_000);
			while (true)
			{
				byte[] entry;
				int length;

				while (!iter.GetNext(out entry, out length))
				{
					if (iter.CurrentAddress >= 100_000_000) return Option.None<(string, long, long)>();
				}

				UTF8Encoding encoding = new UTF8Encoding();
				await iter.WaitAsync();
				nextAddress = iter.NextAddress;
				return Option.Some((encoding.GetString(entry), iter.CurrentAddress, iter.NextAddress));  // Possible to pipe
			}
		}

		public async Task<List<(string, long, long)>> GetList()
		{
			var result = new List<(string, long, long)>();
			using (FasterLogScanIterator iter = logger.Scan(nextAddress, 100_000_000))
			{
				int i = 0;
				byte[] entry;
				int length;
				while (iter.GetNext(out entry, out length))
				{
					UTF8Encoding encoding = new UTF8Encoding();
					if (iter.CurrentAddress >= 1568) Debugger.Break();
					await iter.WaitAsync();
					result.Add((encoding.GetString(entry), iter.CurrentAddress, iter.NextAddress));
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

		public async IAsyncEnumerable<(string, long, long)> GetListAsync()
		{
			using (FasterLogScanIterator iter = logger.Scan(nextAddress, 100_000_000))
			{
				int i = 0;
				await foreach ((byte[] bytes, int length) in iter.GetAsyncEnumerable())
				{
					if (i > 50)
					{
						nextAddress = iter.NextAddress;
						break;
					}

					CancellationTokenSource cts = new CancellationTokenSource();
					UTF8Encoding encoding = new UTF8Encoding();

					try
					{
						await Task.WhenAny(WaitAsync(iter, cts.Token), SetTimeout(cts));
						i++;
					}
					catch (Exception)
					{
						break;
					}

					yield return (encoding.GetString(bytes), iter.CurrentAddress, iter.NextAddress);
				}
			}
		}

		// TODO: Currenly the last items are omitted with this variant. Use Listen() instead.
		public async IAsyncEnumerable<(string, long, long)> ListenAsync_(int flushSize)
		{
			// Always start from beginning. Assume it is refilled or truncate works.
			using (FasterLogScanIterator iter = logger.Scan(0, 1_000_000_000, name: "listen"))
			{
				int i = 0;
				await foreach ((byte[] bytes, int length) in iter.GetAsyncEnumerable())
				{
					if (i >= flushSize)
					{
						nextAddress = iter.NextAddress;
						break;
					}

					CancellationTokenSource cts = new CancellationTokenSource();
					UTF8Encoding encoding = new UTF8Encoding();

					i++;

					// Probably shouldn't wait
					// - https://microsoft.github.io/FASTER/docs/fasterlog#iteration

					yield return (encoding.GetString(bytes), iter.CurrentAddress, iter.NextAddress);
				}
			}
		}

		public List<(string, long, long)> Listen(int flushSize, Cursor cursor)
		{
			List<(string, long, long)> entries = new List<(string, long, long)>();
			// CancellationTokenSource cts = new CancellationTokenSource();
			UTF8Encoding encoding = new UTF8Encoding();

			int i = 0;
			using (FasterLogScanIterator iter = logger.Scan(cursor.NextAddress, 10_000))
			{
				while (true)
				{
					byte[] bytes;
					while (!iter.GetNext(out bytes, out int entryLength))
					{
						// TODO: Cursor keeps an item at the end for some reason. Fix this!
						if (entries.Count > 1)
						{
							return entries;
						}
						return new List<(string, long, long)>();
						// TODO: for the moment make sure to return the final items instead of awaiting more results.
						// if (iter.CurrentAddress >= 1_000_000_000) break;
						// await iter.WaitAsync(cts.Token);
					}

					cursor.Set(iter.NextAddress);
					entries.Add((encoding.GetString(bytes), iter.CurrentAddress, iter.NextAddress));

					i++;

					if (i >= flushSize)
					{
						return entries;
					}
				}
			}
		}

		private async Task SetTimeout(CancellationTokenSource cancellationTokenSource)
		{
			await Task.Delay(300);
			cancellationTokenSource.Cancel();
		}

		private async Task<bool> WaitAsync(FasterLogScanIterator iter, CancellationToken cancellationToken)
		{
			return await iter.WaitAsync(cancellationToken);
		}
		#endregion
	}
}
