using FASTER.core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service_Kestrel.Handlers
{
	class CommitLoop
	{
		//private Instant start;
		//private Duration timeout;
		// private FasterLog log;

		public CommitLoop()
		{
			//start = SystemClock.Instance.GetCurrentInstant();
			//timeout = Duration.FromSeconds(60);
			// this.log = log;
		}

		public FasterLog Log { get; }

		// This method is called by the timer delegate.
		public void Execute(object stateInfo)
		{
			// var now = SystemClock.Instance.GetCurrentInstant();
			CommitState state = stateInfo as CommitState;

			//if (!state.IsCancellationRequested)
			//{
			state.Log.Commit();
			//}
		}
	}

	// https://docs.microsoft.com/en-us/dotnet/api/system.threading.timercallback?view=netframework-4.8
	class CommitState
	{
		private CancellationToken token;
		private FasterLog log;

		public CommitState(FasterLog log, CancellationToken token)
		{
			this.log = log;
			this.token = token;
		}

		public bool IsCancellationRequested => token.IsCancellationRequested;
		public FasterLog Log => log;
	}
}
