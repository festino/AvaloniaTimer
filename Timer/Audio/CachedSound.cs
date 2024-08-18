using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Timer.Audio;

public class CachedSound
{
	public float[] AudioData { get; private set; }
	public WaveFormat WaveFormat { get; private set; }
	public CachedSound(string audioFileName)
	{
		using (var audioFileReader = new AudioFileReader(audioFileName))
		{
			// TODO: could add resampling in here if required
			WaveFormat = audioFileReader.WaveFormat;
			var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
			var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
			int samplesRead;
			while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
			{
				wholeFile.AddRange(readBuffer.Take(samplesRead));
			}
			AudioData = wholeFile.ToArray();
		}
	}

	public static CachedSound? TryCacheSound(string audioFileName)
	{
		try
		{
			return new CachedSound(audioFileName);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
