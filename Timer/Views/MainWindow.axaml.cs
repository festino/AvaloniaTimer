using Avalonia.Controls;
using Avalonia.Interactivity;
using Timer.ViewModels;

namespace Timer.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ShowSettingsButton_Click(object sender, RoutedEventArgs e)
		{
			var settings = ((MainWindowViewModel)DataContext).Settings;
			var settingsWindow = new SettingsWindow(new SettingsWindowViewModel(settings));
			settingsWindow.Show();
		}
	}
}