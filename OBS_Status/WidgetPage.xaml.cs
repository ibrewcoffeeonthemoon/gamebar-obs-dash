using System;
using System.Diagnostics;
using OBS_Status.WebSocket;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OBS_Status
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPage : Page
    {
		public WidgetPage()
        {
            // init UI
            this.InitializeComponent();
            Debug.WriteLine("WidgetPage initialized.");
            // register onLoaded handler
            this.Loaded += OnLoaded;
        }
        ~WidgetPage()
        {
            Debug.WriteLine("WidgetPage destroyed.");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // init Client Instance only once onloaded
            Debug.WriteLine("Client instance initialized");
			// enables automatic updates for ALL bindings in XAML
			this.DataContext = Client.Instance;
            //
            Client.Instance.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(Client.Instance.IsConnected))
                {
                    Debug.WriteLine($"IsConnected changed: {Client.Instance.IsConnected}");
                }
            };
			// then connect once
			await Client.Instance.Connect();
		}
	}

	public class BoolToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool isConnected && isConnected)
			{
                Debug.WriteLine(value);
				return new SolidColorBrush(Colors.LightGreen);
			}

			return new SolidColorBrush(Colors.DarkSlateBlue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
