﻿using NAudio.Wave;

namespace Timer.Audio;

public class AutoDisposeFileReader : ISampleProvider

{
	private readonly AudioFileReader _reader;
	private bool _isDisposed;

	public WaveFormat WaveFormat { get; init; }

	public AutoDisposeFileReader(AudioFileReader reader)
	{
		this._reader = reader;
		this.WaveFormat = reader.WaveFormat;
	}

	public int Read(float[] buffer, int offset, int count)
	{
		if (_isDisposed)
			return 0;
		int read = _reader.Read(buffer, offset, count);
		if (read == 0)
		{
			_reader.Dispose();
			_isDisposed = true;
		}
		return read;
	}
}