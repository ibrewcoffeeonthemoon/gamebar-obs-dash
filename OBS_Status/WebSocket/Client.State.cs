using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace OBS_Status.WebSocket
{
    public partial class Client
    {
		// State
		private bool _isConnected;
		public bool IsConnected
		{
			get => _isConnected;
			set
			{
				if (_isConnected != value)
				{
					_isConnected = value;

					// Manual, safe UI update using stored page reference
					Page?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					{
						Page.StatusBorder.Background = new SolidColorBrush(value ? Colors.LightGreen : Colors.DarkSlateBlue);
						// Add more if you have text or dot:
						// Page.statusTextBlock.Text = value ? "Connected" : "Disconnected";
						// Page.statusDot.Fill = new SolidColorBrush(value ? Colors.LightGreen : Colors.DarkSlateBlue);
					});
				}
			}
		}

    }
}
