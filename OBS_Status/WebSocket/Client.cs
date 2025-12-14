using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OBS_Status.WebSocket
{
	public partial class Client
	{
		private const string ServerAddress = "127.0.0.1";
		private const string ServerPort = "4455";
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
						Identify(message.D);
						break;

					case OpCode.Identified:
						Debug.WriteLine("Successfully identified with OBS-WebSocket!");
						// Connection is now ready for requests
						IsConnected = true;
						break;

					case OpCode.Event:
						string eventType = message.D["eventType"]?.ToString() ?? "Unknown";
						Debug.WriteLine($"Event received: {eventType}");
						// Handle specific events if needed
						// - RecordStateChanged
						if (eventType == "RecordStateChanged")
						{
							// The actual recording state is in eventData.outputActive (true/false)
							IsRecording = message.D["eventData"]?["outputActive"]?.ToObject<bool>() ?? false;

							// Also start recording timer polling
							if (IsRecording)
							{
								StartRecordingTimerPolling();
							}
							else
							{
								StopRecordingTimerPolling();
							}
						}
						break;

					case OpCode.RequestResponse:
						string requestId = message.D["requestId"]?.ToString();
						string requestType = message.D["requestType"]?.ToString();
						bool requestSuccess = message.D["requestStatus"]?["result"]?.ToObject<bool>() ?? false;
						string status = requestSuccess ? "Success" : "Failed";
						Debug.WriteLine($"Response for {requestType} (ID: {requestId}): {status}");

						// handle specific request responses
						if (requestType == "GetRecordStatus" && requestSuccess)
						{
							// Extract the timecode (e.g., "00:01:23.456")
							string timecode = message.D["responseData"]?["outputTimecode"]?.ToString() ?? "00:00:00";
							// Extract the duration in milliseconds for accurate rounding
							long durationMs = message.D["responseData"]?["outputDuration"]?.ToObject<long>() ?? 0;
							// Round to nearest second
							long roundedSeconds = (durationMs + 500) / 1000;  // +500 for proper rounding
							// Convert to TimeSpan and format without milliseconds
							TimeSpan ts = TimeSpan.FromSeconds(roundedSeconds);
							string formattedTime = ts.ToString(@"hh\:mm\:ss");
							Debug.WriteLine($"Raw timecode: {timecode}, Formatted timecode: {formattedTime}");
							// Update the recording timer text
							Client.Instance.RecordingTimecode = formattedTime;

							Debug.WriteLine($"Recording timer updated: {timecode}");
						}
						break;

					default:
						Debug.WriteLine($"Unhandled or future opcode: {message.Op}");
						break;
				}
			}
		}
	}
}

