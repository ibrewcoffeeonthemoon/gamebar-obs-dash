using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace OBS_Status.WebSocket
{
    public partial class Client
    {
		private bool _isRecording;
		public bool IsRecording
		{
			get => _isRecording;
			set
			{
				// update internal state
				_isRecording = value;

				// Only update recording state color if already connected
				if (IsConnected)
				{
					Page?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					{
						Page.StatusBorder.Background = new SolidColorBrush(value ? Colors.Red : Colors.DarkGreen);
					});
				}
			}
		}
	}
}
