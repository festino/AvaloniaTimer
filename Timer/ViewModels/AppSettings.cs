using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Linq;
using Timer.Audio;

namespace Timer.ViewModels;

public partial class AppSettings : ObservableObject
{
	[ObservableProperty]
	private Color _background = new(0, 0, 0, 0);
	[ObservableProperty]
	private Color _foreground = new(255, 0, 0, 0);
	[ObservableProperty]
	private FontFamily _fontFamily =
		FontManager.Current.SystemFonts
			.FirstOrDefault(f => f.Name.Contains("Consolas"), FontFamily.Default);
	[ObservableProperty]
	private double _timeSize = 40;
	[ObservableProperty]
	private bool _msIsEnabled = true;
	[ObservableProperty]
	private double _msSize = 30;

	[ObservableProperty]
	private string? _timerFinishedSoundFilepath = null;
	[ObservableProperty]
	private CachedSound? _timerFinishedSound = null;
	[ObservableProperty]
	private float _timerFinishedSoundVolume = 1.0f;

	public AppSettings()
	{
		PropertyChanged += OnAppSettingsChanged;
	}

	public void OnAppSettingsChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(TimerFinishedSoundFilepath))
			TryLoadSound(TimerFinishedSoundFilepath);
	}

	private void TryLoadSound(string? filepath)
	{
		// may be a bad idea to load it here
		TimerFinishedSoundFilepath = filepath;
		if (filepath is not null)
			TimerFinishedSound = CachedSound.TryCacheSound(filepath);
	}
}
