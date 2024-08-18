using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Linq;
using Timer.ViewModels;

namespace Timer.Views;

public partial class SettingsWindow : Window
{
	private static readonly FilePickerFileType SoundFileTypes =
		new("Audio") { Patterns = ["*.mp3", "*.wav", "*.aiff", "*.aif"] };

	public SettingsWindow() : this(new SettingsWindowViewModel()) { }

	public SettingsWindow(SettingsWindowViewModel settings)
	{
		InitializeComponent();
		DataContext = settings;
		// dropdown constant width
		// how can I add a scrollbar?
		FontsComboBox.ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel());
	}

	private void ChooseTimerFinishedSoundButton_OnClick(object? sender, RoutedEventArgs e)
	{
		var filePaths =
			StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
			{
				FileTypeFilter = [SoundFileTypes],
				AllowMultiple = false
			}).Result;

		if (filePaths.Count != 1)
			return;

		var filepath = filePaths.Single();
		((SettingsWindowViewModel)DataContext).Settings.TimerFinishedSoundFilepath = filepath.Path.LocalPath;
	}

	private void OnClosed(object? sender, EventArgs e)
	{
		((SettingsWindowViewModel)DataContext).OnClosed();
	}
}