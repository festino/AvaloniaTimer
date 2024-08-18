using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Linq;
using Timer.Audio;

namespace Timer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	// Windows clock has 15 ms resolution
	private TimeSpan TimeStep => Settings.MsIsEnabled ? TimeSpan.FromMilliseconds(17) : TimeSpan.FromSeconds(1);

	[ObservableProperty]
	private TimeSpan _time = TimeSpan.Zero;
	[ObservableProperty]
	private TimeSpan _resetTime = TimeSpan.Zero;
	[ObservableProperty]
	private bool _isTimer = false;

	private System.Timers.Timer? _timer = null;
	public bool IsTicking
	{
		get => _timer is not null;
		set => TogglePlay();
	}

	public AppSettings Settings { get; init; } = new();

	public bool IsNoSound => Settings.TimerFinishedSound is null;

	[RelayCommand]
	public void Reset()
	{
		Time = ResetTime;
	}

	[RelayCommand]
	public void TogglePlay()
	{
		if (_timer is null)
			Start();
		else
			Stop();
	}

	public MainWindowViewModel()
	{
		Settings.PropertyChanged += AppSettingsChanged;
		// temporary: TODO load from the config
		Settings.TimerFinishedSoundVolume = 1.0f;
		Settings.TimerFinishedSoundFilepath = "Assets/time_out.mp3";
		Settings.FontFamily = FontManager.Current.SystemFonts
			.FirstOrDefault(f => f.Name.Equals("Minecraft Rus"), Settings.FontFamily);
	}

	public void AppSettingsChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(AppSettings.TimerFinishedSound))
			OnPropertyChanged(nameof(IsNoSound));

		if (args.PropertyName == nameof(AppSettings.MsIsEnabled))
		{
			if (_timer is not null)
				_timer.Interval = TimeStep.TotalMilliseconds;
		}

		if (args.PropertyName == nameof(AppSettings.TimerFinishedSoundVolume))
			AudioPlaybackEngine.Instance.SetVolume(Settings.TimerFinishedSoundVolume);
	}

	private void Tick()
	{
		if (IsTimer)
		{
			var time = Time - TimeStep;
			if (time.TotalSeconds > 0.0)
			{
				Time = time;
				return;
			}
			Time = TimeSpan.Zero;

			OnTimerFinished();
			Stop();
		}
		else
		{
			Time += TimeStep;
		}
	}

	private void Start()
	{
		_timer = new System.Timers.Timer(TimeStep.TotalMilliseconds);
		_timer.Elapsed += (sender, e) => Tick();
		_timer.Start();
		OnPropertyChanged(nameof(IsTicking));
	}

	private void Stop()
	{
		_timer?.Stop();
		_timer = null;
		OnPropertyChanged(nameof(IsTicking));
	}

	private void OnTimerFinished()
	{
		if (Settings.TimerFinishedSound is not null)
			AudioPlaybackEngine.Instance.PlaySound(Settings.TimerFinishedSound);
	}
}