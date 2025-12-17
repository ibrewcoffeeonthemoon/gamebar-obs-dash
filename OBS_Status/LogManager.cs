using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBS_Status
{
	public static class LogManager
	{
		// In-memory log buffer
		private static readonly List<string> _lines = new List<string>();
		// Event fired when a new line is added
		public static event Action<string> LineAdded;

		public static void Add(string message)
		{
			// Add a new line to the log
			_lines.Add(message);
			// Fire event, notifying subscribers
			LineAdded?.Invoke(message);
		}

		// Get a snapshot of the current log lines
		public static IEnumerable<string> Snapshot() => _lines;
	}

}
