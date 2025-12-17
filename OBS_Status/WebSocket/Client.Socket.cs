using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OBS_Status.WebSocket
{
    public partial class Client
    {
		private MessageWebSocket socket;
		private DataWriter writer;

		private async Task SendMessageAsync(string message)
		{
			try
			{
				// Prepare the message to send
				writer.WriteString(message);
				await writer.StoreAsync();  // Asynchronously send the message
				await writer.FlushAsync();  // Ensure the message is fully sent
			}
			catch (Exception ex)
			{
				Log("Error while sending message: " + ex.Message);
				IsConnected = false; // Mark as disconnected on error
			}
		}

		private async Task Ping()
		{
			// Create the Ping/Identify message
			string requestId = Guid.NewGuid().ToString();
			var requestData = new JObject();
			var message = new Message
			{
				Op = (int)OpCode.Request,
				D = new JObject
				{
					["requestType"] = "GetVersion",
					["requestId"] = requestId,
					["requestData"] = requestData,
				}
			};
			string jsonToSend = JsonConvert.SerializeObject(message);
			// Send the Ping/Identify message
			await SendMessageAsync(jsonToSend);
		}
    }
}
