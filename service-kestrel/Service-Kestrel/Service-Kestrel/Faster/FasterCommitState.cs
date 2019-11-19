using FASTER.core;
using System.Threading;

namespace Service_Kestrel.Handlers
{
	// https://docs.microsoft.com/en-us/dotnet/api/system.threading.timercallback?view=netframework-4.8
	class FasterCommitState
	{
		private CancellationToken token;

		public FasterCommitState(FasterLog log, CancellationToken token)
		{
			this.token = token;
			Log = log;
		}

		public bool IsCancellationRequested => token.IsCancellationRequested;
        public FasterLog Log { get; }
    }
}
