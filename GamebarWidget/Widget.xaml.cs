using System;
using Windows.ApplicationModel.AppService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GamebarOBSDash
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Widget1 : Page
    {
		AppServiceConnection connection;
        public Widget1()
        {
            this.InitializeComponent();
			InitializeAppServiceConnection();
		}

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }

		async void InitializeAppServiceConnection()
		{
			connection = new AppServiceConnection
			{
				AppServiceName = "com.example.gamebar.echo",
				PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
			};

			var status = await connection.OpenAsync();
			if (status != AppServiceConnectionStatus.Success) return;

			connection.RequestReceived += OnRequestReceived;
			connection.ServiceClosed += (s, e) => { };
		}

		private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
		{
			var deferral = args.GetDeferral();
			var message = args.Request.Message["text"] as string;
			await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				txtOutput.Text = "From App: " + message;
			});
			deferral.Complete();
		}
	}
}
