using System;
using System.Threading.Tasks;

namespace MinMQ.ScanConsole
{
	public class Program
    {
		public static async Task Main(string[] args)
		{
			string devicePath = "K:\\hlog.log";

			if (args.Length > 0)
			{
				Console.Error.WriteLine("Options not supported ATM");
			}

			var timer = new PrintSystemClockTimer();
			timer.StartTimer();

			var reader = new FasterLogReader();
			await reader.StartScan(devicePath);

			Console.WriteLine("Done!");
        }
    }
}
