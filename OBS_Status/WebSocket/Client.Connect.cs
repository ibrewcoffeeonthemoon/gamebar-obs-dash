using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace OBS_Status.WebSocket
{
    public partial class Client
	{
		// State
		private bool _isConnected;
		public bool IsConnected
		{
			get => _isConnected;
			set
			{
				// update internal state
				_isConnected = value;
				// then call widget page to update color
				widgetPage?.UpdateColor();
			}
		}

		public async Task Connect()
		{
			// Check if already connected
			if (IsConnected)
			{
				Debug.WriteLine("WebSocket is already connected.");
				IsConnected = true;
				return;  // Don't connect again
			}
			try
			{
				// create the websocket
				socket = new MessageWebSocket();
				writer = new DataWriter(socket.OutputStream);
				socket.Control.MessageType = SocketMessageType.Utf8;
				// register message handler
				socket.MessageReceived += OnMessageReceive;
				// connect to the obs server
				string address = ApplicationData.Current.LocalSettings.Values["ObsAddress"] as string;
				string port = ApplicationData.Current.LocalSettings.Values["ObsPort"] as string;
				string endpoint = $"ws://{address}:{port}";
				Log($"Connecting to OBS at {endpoint}...");
				await socket.ConnectAsync(new Uri(endpoint));
			}
			catch (Exception ex)
			{
				Debug.WriteLine("[WebSocket] " + ex.Message);
			}
		}

		private async void Identify(JToken helloData)
		{
			var identifyD = new JObject
			{
				["rpcVersion"] = 1,
				["eventSubscriptions"] =
					(int)EventSubscription.Outputs | // For recording status
					(int)EventSubscription.Config | // For profile changes
					(int)EventSubscription.Scenes // For scene changes
			};

			// Check if authentication is required
			var authObject = helloData["authentication"];
			if (authObject != null)
			{
				string salt = authObject["salt"]?.ToString();
				string challenge = authObject["challenge"]?.ToString();

				if (string.IsNullOrEmpty(salt) || string.IsNullOrEmpty(challenge))
				{
					Debug.WriteLine("Authentication required but missing salt/challenge!");
					// Handle error (close connection, etc.)
					return;
				}

				// Step 1: secret = base64( SHA256( password + salt ) )
				string password = ApplicationData.Current.LocalSettings.Values["ObsPassword"] as string;
				string secretString = password + salt;
				byte[] secretBytes = Encoding.UTF8.GetBytes(secretString);
				byte[] secretHash = SHA256.Create().ComputeHash(secretBytes);
				string base64Secret = Convert.ToBase64String(secretHash);

				// Step 2: auth = base64( SHA256( base64Secret + challenge ) )
				string authInput = base64Secret + challenge;
				byte[] authBytes = Encoding.UTF8.GetBytes(authInput);
				byte[] authHash = SHA256.Create().ComputeHash(authBytes);
				string authenticationString = Convert.ToBase64String(authHash);

				// Add to Identify payload
				identifyD["authentication"] = authenticationString;
				Debug.WriteLine("Authentication computed and added.");
			}
			else
			{
				Debug.WriteLine("No authentication required.");
			}

			// Build the full Identify message
			var identifyMessage = new Message
			{
				Op = (int)OpCode.Identify,
				D = identifyD
			};

			// Serialize to JSON
			string jsonToSend = JsonConvert.SerializeObject(identifyMessage);
			Debug.WriteLine("Sending Identify → " + jsonToSend);

			// Send it over the WebSocket
			SendMessageAsync(jsonToSend).Wait();

			Debug.WriteLine("Identify message sent. Waiting for 'Identified'...");
		}
	}
}
