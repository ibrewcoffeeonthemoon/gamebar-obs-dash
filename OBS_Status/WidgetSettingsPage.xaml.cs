using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		// Local collection that the ListView binds to
		private ObservableCollection<string> _localLogs = new ObservableCollection<string>();

		public WidgetSettingsPage()
        {
            // init UI
            this.InitializeComponent();
			// load form settings
			LoadSettings();
			// load existing log lines
			LogListView.ItemsSource = _localLogs;
			foreach (var line in LogManager.GetHistory())
				_localLogs.Add(line);
			// subscribe to WidgetLog buffer 
			LogManager.LineAdded += OnLogAdded;
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

		private void OnLogAdded(string message)
		{
			// Safety check for the Dispatcher (the "Zombie" check)
			try
			{
				_ = Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					// Add the new line to the UI collection
					_localLogs.Add(message);
				});
			}
			catch (InvalidComObjectException)
			{
				// Dispatcher is no longer valid, unsubscribe from log events
				LogManager.LineAdded -= OnLogAdded;
				Debug.WriteLine("OnLogAdded: Dispatcher is no longer valid, unsubscribed from log events.");
			}
			catch (Exception ex)
			{
				// Log other exceptions
				Debug.WriteLine("OnLog exception: " + ex.Message);
			}
		}
	}
}
