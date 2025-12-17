using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OBS_Status.WebSocket;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OBS_Status
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetSettingsPage : Page
    {
        public WidgetSettingsPage()
        {
            // init UI
            this.InitializeComponent();
			// load form settings
			LoadSettings();
			// load existing log lines
			LogTextBox.Text = string.Join("\n", LogManager.Snapshot());
			// subscribe to WidgetLog buffer 
			LogManager.LineAdded += OnLog;
			// store reference to this page in Client singleton
			Client.Instance.widgetSettingsPage = this;
        }

		void LoadSettings()
		{
			var settings = ApplicationData.Current.LocalSettings;

			AddressTextBox.Text = settings.Values["ObsAddress"] as string ?? "127.0.0.1";
			PortTextBox.Text = settings.Values["ObsPort"] as string ?? "4455";
			PasswordBox.Password = settings.Values["ObsPassword"] as string ?? "";
		}

		private void OnSave(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
			var settings = ApplicationData.Current.LocalSettings;

			settings.Values["ObsAddress"] = AddressTextBox.Text?.Trim();
			settings.Values["ObsPort"] = PortTextBox.Text?.Trim();
			settings.Values["ObsPassword"] = PasswordBox.Password;
		}

		private void OnFocus(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox tb)
				tb.SelectAll();
			else if (sender is PasswordBox pwb)
				pwb.SelectAll();
		}

		private void OnLog(string msg)
		{
			try
			{
				// Update UI on the appropriate thread
				_ = Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					LogTextBox.Text += msg + "\n";
				});
			}
			catch (InvalidComObjectException)
			{
				// Dispatcher is no longer valid, unsubscribe from log events
				LogManager.LineAdded -= OnLog;
				Debug.WriteLine("OnLog: Dispatcher is no longer valid, unsubscribed from log events.");
			}
			catch (Exception ex)
			{
				// Log other exceptions
				Debug.WriteLine("OnLog exception: " + ex.Message);
			}
		}
	}
}
