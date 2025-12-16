using System.Diagnostics;
using Windows.UI.Xaml.Controls;

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
            this.InitializeComponent();
        }

        private void OnSave(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine("Save button clicked in WidgetSettingsPage.");
		}
	}
}
