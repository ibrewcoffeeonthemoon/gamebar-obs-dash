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
            //
            Window.Current.Activated += (object s, WindowActivatedEventArgs e) =>
            {
                Debug.WriteLine($"Window.Current.Visible={Window.Current.Visible}");
                if (Window.Current.Visible)
                {
                    Debug.WriteLine("ENTER: WidgetPage window visible.");
                }
                else
                {
                    Debug.WriteLine("EXIT: WidgetPage window invisible.");
                }
            };
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
					StatusBorder.Background = new SolidColorBrush(Colors.DimGray);
                    return;
                }
                else if (!Client.Instance.IsRecording)
                {
					StatusBorder.Background = new SolidColorBrush(Colors.DarkGreen);
                    return;
				}
                else
                {
					StatusBorder.Background = new SolidColorBrush(Colors.Red);
                    return;
                }
			});

        }
	}
}
