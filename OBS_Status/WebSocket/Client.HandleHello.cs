using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace OBS_Status.WebSocket
{
    public partial class Client
	{
		private async void HandleHello(JToken helloData)
		{
			Debug.WriteLine(helloData);
		}
	}
}
