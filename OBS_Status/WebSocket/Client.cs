using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OBS_Status.WebSocket
{
	public partial class Client
	{
        // The single instance — created lazily (only when first needed)
        private static readonly Lazy<Client> _instance = new Lazy<Client>(() => new Client());
		// Public way to access the single instance
		public static Client Instance => _instance.Value;

		// This will hold the reference to your WidgetPage (or just the dispatcher)
		public WidgetPage widgetPage { get; set; }
		public WidgetSettingsPage widgetSettingsPage { get; set; }

		// Private constructor — no one can create new Client() from outside
		private Client()
		{
		}

		// heartbeat loop
		public async Task Run()
		{
			Log("Client Run(): starting heartbeat loop.");
			while (true)
			{
				// if not connected, try to connect first
				if (!IsConnected)
					await Connect();
				else
				{
					// if connected but missing profile, manually request it
					if (ProfileName == "")
						await RequestGetProfileList();
					// if connected but missing scene, manually request it
					if (SceneName == "")
						await RequestGetSceneList();
					// if connected but not recording, just send ping to check connection
					if (!IsRecording)
						await Ping();
					// if connected and recording, request record status to update timecode
					else
						await RequestGetRecordStatus();
				}
				// delay for 1 second before next iteration
				await Task.Delay(1000);
			}
		}
		public void Log(string message)
		{
			// append message to debug output
			Debug.WriteLine(message);
			// also send to settings page log if available
			//widgetSettingsPage?.LogMessage(message);
			LogManager.AddLog(message);
		}
	}
}

