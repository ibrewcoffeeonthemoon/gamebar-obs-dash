using Newtonsoft.Json;
using Newtonsoft.Json.Linq;  // For JObject/JToken (dynamic handling)

namespace OBS_Status.WebSocket
{
	/// <summary>
	/// Generic envelope for all OBS-WebSocket 5.x messages.
	/// </summary>
	public class Message
	{
		[JsonProperty("op")]
		public int Op { get; set; }

		[JsonProperty("d")]
		public JToken D { get; set; }  // Use JToken for fully dynamic/unknown schema
	}

	public enum OpCode
	{
		Hello = 0,
		Identify = 1,
		Identified = 2,
		Reidentify = 3,
		Event = 5,
		Request = 6,
		RequestResponse = 7,
		RequestBatch = 8,
		RequestBatchResponse = 9
	}
}