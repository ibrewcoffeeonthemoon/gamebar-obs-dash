using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
			catch (ObjectDisposedException ex)
			{
				Debug.WriteLine("Error: Attempted to send on a closed WebSocket: " + ex.Message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error while sending message: " + ex.Message);
			}
		}

    }
}
