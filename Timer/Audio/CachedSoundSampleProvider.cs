using NAudio.Wave;
using System;

namespace Timer.Audio;

public class CachedSoundSampleProvider : ISampleProvider
{
	private readonly CachedSound _cachedSound;
	private long _position;

	public WaveFormat WaveFormat => _cachedSound.WaveFormat;

	public CachedSoundSampleProvider(CachedSound cachedSound)
	{
		this._cachedSound = cachedSound;
	}

	public int Read(float[] buffer, int offset, int count)
	{
		var availableSamples = _cachedSound.AudioData.Length - _position;
		var samplesToCopy = Math.Min(availableSamples, count);
		Array.Copy(_cachedSound.AudioData, _position, buffer, offset, samplesToCopy);
		_position += samplesToCopy;
		return (int)samplesToCopy;
	}
}
