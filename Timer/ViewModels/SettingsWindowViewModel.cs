using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Timer.Audio;

namespace Timer.ViewModels;

public partial class SettingsWindowViewModel : ViewModelBase
{
	private readonly AudioPlaybackEngine _audioPlaybackEngine = AudioPlaybackEngine.Instance;

	public AppSettings Settings { get; init; }

	[ObservableProperty]
	private ObservableCollection<FontFamily> _fonts = [];

	public string TimerFinishedSoundName => Path.GetFileName(Settings.TimerFinishedSoundFilepath ?? "no sound");
	public bool SoundIsReady => Settings.TimerFinishedSound is not null;
	public bool IsPlayingTimerFinishedSound => _audioPlaybackEngine.IsPlaying;

	[RelayCommand]
	public void PlayTimerFinishedSound()
	{
		if (Settings.TimerFinishedSound is not null)
		{
			if (IsPlayingTimerFinishedSound)
				_audioPlaybackEngine.StopSound();
			else
				_audioPlaybackEngine.PlaySound(Settings.TimerFinishedSound);
		}
	}

	public SettingsWindowViewModel() : this(new AppSettings()) { }

	public SettingsWindowViewModel(AppSettings settings)
	{
		Settings = settings;
		Settings.PropertyChanged += AppSettingsChanged;
		_audioPlaybackEngine.PropertyChanged += AudioPlaybackEngineOnPropertyChanged;

		foreach (var font in FontManager.Current.SystemFonts.OrderBy(f => f.Name))
		{
			Fonts.Add(font);
		}
	}

	public void OnClosed()
	{
		Settings.PropertyChanged -= AppSettingsChanged;
		_audioPlaybackEngine.PropertyChanged -= AudioPlaybackEngineOnPropertyChanged;
	}

	void AudioPlaybackEngineOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(AudioPlaybackEngine.IsPlaying))
			OnPropertyChanged(nameof(IsPlayingTimerFinishedSound));
	}

	private void AppSettingsChanged(object? sender, PropertyChangedEventArgs args)
	{
		// propagate event to window bindings
		if (args.PropertyName == nameof(AppSettings.TimerFinishedSoundFilepath))
			OnPropertyChanged(nameof(TimerFinishedSoundName));

		if (args.PropertyName == nameof(AppSettings.TimerFinishedSound))
			OnPropertyChanged(nameof(SoundIsReady));
	}
}
