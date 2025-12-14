using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

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

				// Manual, safe UI update using stored page reference 
				Page?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					Page.StatusBorder.Background = new SolidColorBrush(value ? Colors.DarkGreen : Colors.DarkSlateBlue);
					// Add more if you have text or dot:
					// Page.statusTextBlock.Text = value ? "Connected" : "Disconnected";
					// Page.statusDot.Fill = new SolidColorBrush(value ? Colors.LightGreen : Colors.DarkSlateBlue);
				});
			}
		}

		private string password = File.ReadAllText(Path.Combine(System.AppContext.BaseDirectory, ".password"));

		private async void Identify(JToken helloData)
		{
			var identifyD = new JObject
			{
				["rpcVersion"] = 1,
				["eventSubscriptions"] = (int)EventSubscription.Outputs  // For recording status
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
