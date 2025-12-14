using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OBS_Status.WebSocket
{
	public partial class Client
	{
		public WidgetPage Page { get; set; }

		// Private constructor — no one can create new Client() from outside
		private Client()
		{
			// create the websocket
			socket = new MessageWebSocket();
			writer = new DataWriter(socket.OutputStream);
			socket.Control.MessageType = SocketMessageType.Utf8;
			// register message handler
			socket.MessageReceived += OnMessageReceive;
		}
        // The single instance — created lazily (only when first needed)
        private static readonly Lazy<Client> _instance = new Lazy<Client>(() => new Client());
		// Public way to access the single instance
		public static Client Instance => _instance.Value;
		// This will hold the reference to your WidgetPage (or just the dispatcher)
	}
}

