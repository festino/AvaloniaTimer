using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace Timer.Audio;

/// <summary>
/// See https://markheath.net/post/fire-and-forget-audio-playback-with
/// </summary>
public class AudioPlaybackEngine : ObservableObject, IDisposable
{
	private readonly IWavePlayer _outputDevice;
	private readonly MixingSampleProvider _mixer;

	public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);

	private int _inputCount = 0;
	private int InputCount
	{
		get => _inputCount;
		set
		{
			_inputCount = value;
			OnPropertyChanged(nameof(IsPlaying));
		}
	}

	public bool IsPlaying => InputCount > 0;

	public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
	{
		_outputDevice = new WaveOutEvent();
		_mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
		_mixer.MixerInputEnded += (sender, args) => InputCount--;
		_mixer.ReadFully = true;
		_outputDevice.Init(_mixer);
		_outputDevice.Play();
	}

	public void PlaySound(string fileName)
	{
		var input = new AudioFileReader(fileName);
		AddMixerInput(new AutoDisposeFileReader(input));
	}

	public void PlaySound(CachedSound sound)
	{
		AddMixerInput(new CachedSoundSampleProvider(sound));
	}

	public void StopSound()
	{
		_mixer.RemoveAllMixerInputs();
		InputCount = 0;
	}

	public void SetVolume(float volume)
	{
		_outputDevice.Volume = volume;
	}

	private void AddMixerInput(ISampleProvider input)
	{
		_mixer.AddMixerInput(ConvertToRightChannelCount(input));
		InputCount++;
	}

	public void Dispose()
	{
		_outputDevice.Dispose();
	}

	private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
	{
		if (input.WaveFormat.Channels == _mixer.WaveFormat.Channels)
		{
			return input;
		}
		if (input.WaveFormat.Channels == 1 && _mixer.WaveFormat.Channels == 2)
		{
			return new MonoToStereoSampleProvider(input);
		}
		throw new NotImplementedException("Not yet implemented this channel count conversion");
	}
}