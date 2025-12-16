using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;


namespace OBS_Status.WebSocket
{
	public partial class Client
	{
		// state
		private bool _isRecording;
		public bool IsRecording
		{
			get => _isRecording;
			set
			{
				// update internal state
				_isRecording = value;
				// then call widget page to update color
				widgetPage?.UpdateColor();
			}
		}

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
					widgetPage?.UpdateRecordingTimer();
			}
		}

		private string _profileName = "";
		public string ProfileName
		{
			get => _profileName;
			set
			{
				// update internal state
				_profileName = value;

				// Only update profile name text if already connected
				if (IsConnected)
					widgetPage?.UpdateProfileName();
			}
		}

		private string _sceneName = "";
		public string SceneName
		{
			get => _sceneName;
			set
			{
				// update internal state
				_sceneName = value;
				// Only update scene name text if already connected
				if (IsConnected)
					widgetPage?.UpdateSceneName();
			}
		}

		private async Task RequestGetRecordStatus()
		{
			// Build the GetRecordStatus request
			string requestId = Guid.NewGuid().ToString();
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
			}
		}
		private async Task RequestGetProfileList()
		{
			// Build the GetProfileList request
			string requestId = Guid.NewGuid().ToString();
			var requestData = new JObject();  // No parameters needed for GetProfileList
			var d = new JObject
			{
				["requestType"] = "GetProfileList",
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
				Debug.WriteLine($"Failed to send GetProfileList: {ex.Message}");
			}
		}
		private async Task RequestGetSceneList()
		{
			// Build the GetSceneList request
			string requestId = Guid.NewGuid().ToString();
			var requestData = new JObject();  // No parameters needed for GetSceneList
			var d = new JObject
			{
				["requestType"] = "GetSceneList",
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
				Debug.WriteLine($"Failed to send GetSceneList: {ex.Message}");
			}
		}

	}
}
