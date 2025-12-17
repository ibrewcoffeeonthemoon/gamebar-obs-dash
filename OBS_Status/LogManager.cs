using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBS_Status
{
	public static class LogManager
	{
		// The master history
		private static readonly List<string> _history = new List<string>();

		// The event that the UI will listen to
		public static event Action<string> LineAdded;

		public static void AddLog(string message)
		{
			// Add to the master history, locking to avoid race conditions
			lock (_history)
			{
				_history.Add(message);
			}
			// Tell the UI a new line is ready
			LineAdded?.Invoke(message);
		}

		public static List<string> GetHistory()
		{
			// Return a copy of the master history, locking to avoid race conditions
			lock (_history)
			{
				return new List<string>(_history);
			}
		}
	}
}
