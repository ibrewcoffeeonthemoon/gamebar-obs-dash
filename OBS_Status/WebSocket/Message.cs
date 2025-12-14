using System;
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

	[Flags]
	public enum EventSubscription : uint
	{
		None = 0,
		General = 1 << 0,
		Config = 1 << 1,
		Scenes = 1 << 2,
		Inputs = 1 << 3,
		Transitions = 1 << 4,
		Filters = 1 << 5,
		Outputs = 1 << 6,        // ← This one for recording/streaming state
		MediaInputs = 1 << 7,
		Vendors = 1 << 8,
		UI = 1 << 9,
		All = 0xFFFFFFFF
	}
}