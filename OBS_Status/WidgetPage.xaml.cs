using System.Diagnostics;
using OBS_Status.WebSocket;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            // register onloaded handler
            this.Loaded += OnLoaded;
            // store reference to this page in Client singleton
            Client.Instance.Page = this; 
		}
        ~WidgetPage()
        {
            Debug.WriteLine("WidgetPage destroyed.");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Client.Instance.Run();
		}

        public async void UpdateColor()
        {
			_ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
                if (!Client.Instance.IsConnected)
                {
					StatusButton.Background = new SolidColorBrush(Colors.DimGray);
                    return;
                }
                else if (!Client.Instance.IsRecording)
                {
					StatusButton.Background = new SolidColorBrush(Colors.DarkGreen);
                    return;
				}
                else
                {
					StatusButton.Background = new SolidColorBrush(Colors.Red);
                    return;
                }
			});

        }

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            bool infoIsVisible = InfoTextPanel.Visibility == Visibility.Visible;
			// clicking button toggle visibility of info text panel
			InfoTextPanel.Visibility = infoIsVisible ? Visibility.Collapsed : Visibility.Visible;
            // resize the window to fit content
            int minW = 120;
            int maxW = 400;
            StatusButton.MinWidth = infoIsVisible ? minW : maxW;
            StatusButton.MaxWidth = infoIsVisible ? minW : maxW;
		}

	}
}
