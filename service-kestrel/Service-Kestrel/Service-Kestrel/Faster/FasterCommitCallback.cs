using FASTER.core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Kestrel.Handlers
{
	class FasterCommitCallback
	{
		public FasterLog Log { get; }

		public void Execute(object stateInfo)
		{
			FasterCommitState state = stateInfo as FasterCommitState;

			if (!state.IsCancellationRequested)
			{
				state.Log.Commit();
			}
		}
	}
}
