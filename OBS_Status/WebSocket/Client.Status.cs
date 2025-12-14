using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;

namespace OBS_Status.WebSocket
{
	public partial class Client
	{
		private string _recordingTimecode = "00:00:00";
		public string RecordingTimecode
		{
			get => _recordingTimecode;
			set
			{
				// update internal state
				_recordingTimecode = value;

				// Only update recording timer text if already connected
				if (IsConnected)
				{
					Page?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					{
						Page.RecordingTimerText.Text = value;
					});
				}
			}
		}

		private Timer recordingTimer;
		private async void StartRecordingTimerPolling()
		{
			// stop any existing timer
			StopRecordingTimerPolling();

			// start a new timer to poll every second
			recordingTimer = new Timer(async _ =>
			{
				Debug.WriteLine(socket.Information.ToString());
				
				// Build the GetRecordStatus request
				//string requestId = Guid.NewGuid().ToString();
				string requestId = "f819dcf0-89cc-11eb-8f0e-382c4ac93b9c";
				var requestData = new JObject();  // No parameters needed for GetRecordStatus
				var d = new JObject
				{
					["requestType"] = "GetRecordStatus",
					["requestId"] = requestId,
					["requestData"] = requestData
				};
				var message = new Message
				{
					Op = (int)OpCode.Request,
					D = d
				};
				string jsonToSend = JsonConvert.SerializeObject(message);

				// Send it over the WebSocket
				try
				{
					await SendMessageAsync(jsonToSend);
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Failed to send GetRecordStatus: {ex.Message}");
					// Optionally stop polling on error
				}
			}, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}

		private void StopRecordingTimerPolling()
		{
			recordingTimer?.Dispose();
			recordingTimer = null;
		}
	}
}
