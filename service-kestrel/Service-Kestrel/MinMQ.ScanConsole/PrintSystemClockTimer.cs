using NodaTime;
using System;
using System.Threading;

namespace MinMQ.ScanConsole
{
	public class PrintSystemClockTimer
	{
		Instant start;

		public PrintSystemClockTimer()
		{
			start = SystemClock.Instance.GetCurrentInstant();
		}

		public void StartTimer()
		{
			var timer = new Timer(WriteProgress, start, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
		}

		public void WriteProgress(object stateInfo)
		{
			if (stateInfo is Instant start)
			{
				var duration = SystemClock.Instance.GetCurrentInstant() - start;
				Console.WriteLine("Uptime={0}", duration.ToTimeSpan());
			}
			else
			{
				DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				Console.WriteLine("State unknown. Time=", SystemClock.Instance.GetCurrentInstant().InZone(tz).TimeOfDay);
			}
		}
	}
}
