using Windows.Foundation.Collections;
using Windows.ApplicationModel.AppService;

class Program
{
	static AppServiceConnection? connection;

	static async Task Main()
	{
		connection = new AppServiceConnection
		{
			AppServiceName = "com.example.gamebar.echo",
			PackageFamilyName = "GamebarOBSDash_8wekyb3d8bbwe"
		};

		connection.RequestReceived += OnRequestReceived;
		var status = await connection.OpenAsync();

		while (true)
		{
			string? input = Console.ReadLine();
			if (input == null)
			{
				break;
			}
			var message = new ValueSet { ["text"] = input };
			await connection.SendMessageAsync(message);
		}
	}

	private static async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
	{
		var deferral = args.GetDeferral();
		var text = args.Request.Message["text"] as string;
		Console.WriteLine($"From Widget: {text}");

		// echo back
		var response = new ValueSet { ["text"] = "Echo: " + text };
		await args.Request.SendResponseAsync(response);
		deferral.Complete();
	}
}