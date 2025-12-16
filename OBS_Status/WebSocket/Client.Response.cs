using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.Networking.Sockets;

namespace OBS_Status.WebSocket
{
	public partial class Client
    {
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
						}
						else if (eventType == "CurrentProfileChanged")
						{
							ProfileName = message.D["eventData"]?["profileName"]?.ToObject<string>() ?? "";
						}
						else if (eventType == "CurrentProgramSceneChanged")
						{
							SceneName = message.D["eventData"]?["sceneName"]?.ToObject<string>() ?? "";
						}
						break;

					case OpCode.RequestResponse:
						string requestId = message.D["requestId"]?.ToString();
						string requestType = message.D["requestType"]?.ToString();
						bool requestSuccess = message.D["requestStatus"]?["result"]?.ToObject<bool>() ?? false;
						string status = requestSuccess ? "Success" : "Failed";
						Debug.WriteLine($"Response for {requestType} (ID: {requestId}): {status}");

						if (!requestSuccess)
							break;
						// handle specific request responses
						if (requestType == "GetRecordStatus")
						{
							// Update recording state
							IsRecording = message.D["responseData"]?["outputActive"]?.ToObject<bool>() ?? false;
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
							RecordingTimecode = formattedTime;
							Debug.WriteLine($"Recording timer updated: {timecode}");
						}
						else if (requestType == "GetProfileList")
						{
							ProfileName = message.D["responseData"]?["currentProfileName"]?.ToObject<string>() ?? "";
						}
						else if (requestType == "GetSceneList")
						{
							SceneName = message.D["responseData"]?["currentProgramSceneName"]?.ToObject<string>() ?? "";
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
