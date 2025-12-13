using System.Diagnostics;
using OBS_Status.WebSocket;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OBS_Status
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPage : Page
    {
		public Border StatusBorder => statusBorder;

		public WidgetPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("WidgetPage initialized.");

            this.Loaded += OnLoaded;

		    Client.Instance.Initialize(this);
        }
        ~WidgetPage()
        {
            Debug.WriteLine("WidgetPage destroyed.");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Client.Instance.Connect();
		}
	}
}
