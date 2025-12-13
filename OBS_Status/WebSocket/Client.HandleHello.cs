using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage.Streams;

namespace OBS_Status.WebSocket
{
    public partial class Client
	{
		private async void Identify(JToken helloData)
		{
			// Build the full Identify message
			var identifyMessage = new Message
			{
				Op = (int)OpCode.Identify,
				// Build the "d" payload for Identify
				D = new JObject
				{
					["rpcVersion"] = 1,  // Always use 1 (current as of OBS-WebSocket 5.x)
					//["eventsubscriptions"] = (int)eventsubscriptions  // optional but highly recommended
																	   //No "authentication" field needed since no password
				}
			};

			// Serialize to JSON
			string jsonToSend = JsonConvert.SerializeObject(identifyMessage);
			Debug.WriteLine("Sending Identify → " + jsonToSend);

			// Send it over the WebSocket
			using (var writer = new DataWriter(socket.OutputStream))
			{
				writer.UnicodeEncoding = UnicodeEncoding.Utf8;
				writer.WriteString(jsonToSend);
				await writer.StoreAsync().AsTask();
				await writer.FlushAsync().AsTask();
			}

			Debug.WriteLine("Identify message sent. Waiting for 'Identified'...");
		}
	}
}
