using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OBS_Status.WebSocket
{
	public partial class Client : INotifyPropertyChanged
	{
		// States
		private bool _isConnected;
		public bool IsConnected
		{
			get => _isConnected;
			set
			{
				if (_isConnected != value)
				{
					_isConnected = value;
					OnPropertyChanged();
				}
			}
		}

		// INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			// This safely raises the event – UWP automatically marshals to UI thread
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
