using OBS_Status.WebSocket;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OBS_Status
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Widget : Page
    {
		Client ws = new Client();

		public Widget()
        {
            this.InitializeComponent();
        }
        private async void MyButton_Click(object sender, RoutedEventArgs e)
        {
			await ws.ConnectAndSendMessageAsync();
		}
    }
}
