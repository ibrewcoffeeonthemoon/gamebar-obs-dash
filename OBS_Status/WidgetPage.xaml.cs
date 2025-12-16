using System;
using System.Diagnostics;
using Microsoft.Gaming.XboxGameBar;
using OBS_Status.WebSocket;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OBS_Status
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPage : Page
    {
		// ref to the XboxGameBarWidget
		private XboxGameBarWidget widget = null;

		public WidgetPage()
        {
            // init UI
            this.InitializeComponent();
            // ensure state dependent UI is in sync if widget is recreated
            UpdateColor();
            UpdateRecordingTimer();
            UpdateProfileName();
            UpdateSceneName();
            // register onloaded handler
            this.Loaded += OnLoaded;
            // store reference to this page in Client singleton
            Client.Instance.widgetPage = this;
		}
        ~WidgetPage()
        {
            Debug.WriteLine("WidgetPage destroyed.");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("WidgetPage OnLoaded(): await Client.Instance.Run()");
            await Client.Instance.Run();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// In our example we pass the XboxGameBarWidget through the Navigate event when creating and showing the parent widget for the first time, your implementation may differ.

			// Here we store the parameter in a member variable "widget":
			widget = e.Parameter as XboxGameBarWidget;

			// Hook up the settings clicked event
			widget.SettingsClicked += Widget_SettingsClicked;

			// ...
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

		private async void Widget_SettingsClicked(XboxGameBarWidget sender, object args)
		{
            // if necessary pre-configure any required data needed by the settings widget prior to activation
            // ...
            Debug.WriteLine("Widget_SettingsClicked: Activating settings widget.");
            await sender.ActivateSettingsAsync();
		}
	}
}
