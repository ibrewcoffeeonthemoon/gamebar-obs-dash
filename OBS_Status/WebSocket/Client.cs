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
		public WidgetPage Page { get; set; }

		// Private constructor — no one can create new Client() from outside
		private Client()
		{
		}

		// heartbeat loop
		public async Task Run()
		{
			while (true)
			{
				// if not connected, try to connect first
				if (!IsConnected)
				{
					await Connect();
				}
				// if connected but not recording, just send ping to check connection
				else if (!IsRecording)
				{
					Debug.WriteLine("connected, not recording, keep sending ping");
				}
				else
				{
					Debug.WriteLine("if already connected and is recording, send GetRecordStatus request");
				}
				// delay for 1 second before next iteration
				await Task.Delay(1000);
			}
		}
	}
}

