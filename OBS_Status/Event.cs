using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBS_Status
{
	public static class GamebarVisibilityChangedEvent
	{
		public static event Action<bool> VisibilityChanged;

		public static void RaiseGamebarVisibilityChanged(bool isVisible)
		{
			VisibilityChanged?.Invoke(isVisible);
		}
	}
}
