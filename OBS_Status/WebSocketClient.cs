using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OBS_Status // <-- match your project namespace
{
	public class WebSocketClient
	{
		private const string ServerAddress = "127.0.0.1";
		private const string ServerPort = "4455";
		private MessageWebSocket ws;
		private DataWriter writer;

		public async Task ConnectAndSendMessageAsync()
		{
			System.Diagnostics.Debug.WriteLine("hello sending ws");

			try
			{
				ws = new MessageWebSocket();
				ws.Control.MessageType = SocketMessageType.Utf8;

				ws.MessageReceived += Ws_MessageReceived;

				Debug.WriteLine("Connecting to OBS...");

				await ws.ConnectAsync(new Uri("ws://127.0.0.1:4455"));

				Debug.WriteLine("Connected!");

				writer = new DataWriter(ws.OutputStream);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("[WebSocket] " + ex.Message);
			}
		}

		private void Ws_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
		{
			using (var reader = args.GetDataReader())
			{
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

