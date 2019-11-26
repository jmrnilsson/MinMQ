using FASTER.core;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinMQ.ScanConsole
{
	public class FasterLogReader
	{
		public async Task<List<(string, long, long)>> StartScan(string devicePath)
		{
			IDevice device = Devices.CreateLogDevice(devicePath);
			FasterLog logger = new FasterLog(new FasterLogSettings { LogDevice = device });
			long nextAddress = 0;
			bool keepGoing = true;
			int i = 0;

			var result = new List<(string, long, long)>();

			// using (FasterLogScanIterator iter = logger.Scan(logger.BeginAddress, 100_000_000, name: nameof(GetListAsync)))
			using (FasterLogScanIterator iter = logger.Scan(nextAddress, 100_000_000))
			{
				while(keepGoing)
				{
					Console.WriteLine("Going");
					LocalTime timeOfDay;
					await foreach ((byte[] bytes, int length) in iter.GetAsyncEnumerable())
					{
						
						DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
						timeOfDay = SystemClock.Instance.GetCurrentInstant().InZone(tz).TimeOfDay;
						nextAddress = iter.NextAddress;
						Console.WriteLine("Time={1} NextAddress={0}, Count={2}", iter.NextAddress, timeOfDay, i++);
						var cts = new CancellationTokenSource();
						ASCIIEncoding ascii = new ASCIIEncoding();

						try
						{
							await Task.WhenAny(WaitAsync(iter), SetTimeout(cts));
						}
						catch (Exception e)
						{
							Console.Error.WriteLine($"Error={e.GetType()}, Message={e.ToString()}");
							break;
						}

						timeOfDay = SystemClock.Instance.GetCurrentInstant().InZone(tz).TimeOfDay;
						Console.WriteLine("Time={2} Content={0}", ascii.GetString(bytes), iter.NextAddress, timeOfDay);
					}
				}

			}

			return result;
		}

		public async Task SetTimeout(CancellationTokenSource cancellationTokenSource)
		{
			await Task.Delay(300);
			cancellationTokenSource.Cancel();
		}

		public async Task<bool> WaitAsync(FasterLogScanIterator iter)
		{
			return await iter.WaitAsync();
		}
	}
}
