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

        public async void UpdateRecordingTimer()
        {
			_ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				RecordingTimerText.Text = Client.Instance.RecordingTimecode;
			});
        }
        public async void UpdateProfileName()
        {
			_ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				ProfileNameText.Text = Client.Instance.ProfileName;
			});
        }
        public async void UpdateSceneName()
        {
			_ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				SceneNameText.Text = Client.Instance.SceneName;
			});
        }

		private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
			// toggle the info visibility
			InfoTextPanel.Visibility = InfoTextPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            bool infoIsVisible = InfoTextPanel.Visibility == Visibility.Visible;
            // change the button alignment
			StatusButton.HorizontalAlignment = infoIsVisible ? HorizontalAlignment.Stretch : HorizontalAlignment.Right; 
            // change the recording timer text alignment
            RecordingTimerPanel.HorizontalAlignment = infoIsVisible ? HorizontalAlignment.Right : HorizontalAlignment.Center;
		}

	}
}
