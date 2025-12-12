using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OBS_Status.WebSocket
{
	public class Client
	{
		private const string ServerAddress = "127.0.0.1";
		private const string ServerPort = "4455";
		private MessageWebSocket ws;
		private DataWriter writer;

		public async Task ConnectAndSendMessageAsync()
		{
			try
			{
				// create the websocket
				ws = new MessageWebSocket();
				ws.Control.MessageType = SocketMessageType.Utf8;

				// register message handler
				ws.MessageReceived += OnMessageReceive;

				// connect to the obs server
				string endpoint = $"ws://{ServerAddress}:{ServerPort}";
				Debug.WriteLine($"Connecting to OBS at {endpoint}...");
				await ws.ConnectAsync(new Uri(endpoint));

				// connection establishd
				Debug.WriteLine("Connected!");
				writer = new DataWriter(ws.OutputStream);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("[WebSocket] " + ex.Message);
			}
		}

		private void OnMessageReceive(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
		{
			using (var reader = args.GetDataReader())
			{
				// read the message
				string msg = reader.ReadString(reader.UnconsumedBufferLength);
				Debug.WriteLine("OBS → " + msg);

				// After receiving "Hello" from OBS, send Identify:
				//if (msg.Contains("\"op\":0")) // Hello
				//{
				//	string identify = "{\"op\":1,\"d\":{\"rpcVersion\":1}}";
				//	writer.WriteString(identify);
				//	writer.StoreAsync();
				//}
			}
		}
	}
}

