using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Networking.Sockets;
using Windows.UI.Xaml.Media;

namespace OBS_Status.WebSocket
{
	public partial class Client
	{
		private static bool isConnected = false;

		private const string ServerAddress = "127.0.0.1";
		private const string ServerPort = "4455";
		private WidgetPage widgetPage;
		private MessageWebSocket socket;

		public Client(WidgetPage widgetPage)
		{
			this.widgetPage = widgetPage;

			// create the websocket
			socket = new MessageWebSocket();
			socket.Control.MessageType = SocketMessageType.Utf8;

			// register message handler
			socket.MessageReceived += OnMessageReceive;

			updateColor();
		}

		public async Task Connect()
		{
			// Check if already connected
			if (isConnected)
			{
				Debug.WriteLine("WebSocket is already connected.");
				return;  // Don't connect again
			}

			try
			{
				// connect to the obs server
				string endpoint = $"ws://{ServerAddress}:{ServerPort}";
				Debug.WriteLine($"Connecting to OBS at {endpoint}...");
				await socket.ConnectAsync(new Uri(endpoint));

				// connection establishd
				Debug.WriteLine("Connected!");
				isConnected = true;

				// change text box color to green
				updateColor();

			}
			catch (Exception ex)
			{
				Debug.WriteLine("[WebSocket] " + ex.Message);
			}
		}

		private void updateColor()
		{
			if (isConnected)
			{
				widgetPage.StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
			}
			else
			{
				widgetPage.StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.DarkSlateBlue);
			}
		}

		private void OnMessageReceive(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
		{
			using (var reader = args.GetDataReader())
			{
				// read the message
				string msg = reader.ReadString(reader.UnconsumedBufferLength);

				// Parse the JSON into our generic ObsMessage object
				Message message = JsonConvert.DeserializeObject<Message>(msg);

				if (message == null)
				{
					Debug.WriteLine("Failed to deserialize OBS message.");
					return;
				}

				// Safely handle the opcode (using int base for future-proofing)
				Debug.WriteLine($"Received opcode: {message.Op}");

				switch ((OpCode)message.Op)
				{
					case OpCode.Hello:
						Debug.WriteLine("Received Hello from OBS-WebSocket.");
						HandleHello(message.D);
						break;

					case OpCode.Identified:
						Debug.WriteLine("Successfully identified with OBS-WebSocket!");
						// Connection is now ready for requests
						break;

					case OpCode.Event:
						string eventType = message.D["eventType"]?.ToString() ?? "Unknown";
						Debug.WriteLine($"Event received: {eventType}");
						// Handle specific events if needed
						break;

					case OpCode.RequestResponse:
						string requestId = message.D["requestId"]?.ToString();
						string requestType = message.D["requestType"]?.ToString();
						string status = message.D["requestStatus"]?["result"]?.ToObject<bool>() ?? false ? "Success" : "Failed";
						Debug.WriteLine($"Response for {requestType} (ID: {requestId}): {status}");
						break;

					default:
						Debug.WriteLine($"Unhandled or future opcode: {message.Op}");
						break;
				}
			}
		}
	}
}

