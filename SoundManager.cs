using NAudio.Wave;
using NVorbis;


using NAudio.Wave;
using NVorbis;

using NAudio.Wave;
using NVorbis;

public class SoundManager
{
    private static readonly Lazy<SoundManager> _instance = new Lazy<SoundManager>(() => new SoundManager());
    public static SoundManager Instance => _instance.Value;

    private IWavePlayer _waveOutDevice;
    private WaveChannel32 _waveChannel;
    private float _volume = 1f;

    private SoundManager()
    {
        // Private constructor to enforce singleton pattern
    }

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            if (_waveChannel != null)
                _waveChannel.Volume = _volume;
        }
    }

    public async Task PlaySoundAsync(string filePath)
    {
        StopCurrentSound(); // Stop any currently playing sound

        await Task.Run(() =>
        {
            // Use NVorbis to open and decode the Ogg Vorbis file
            var vorbisReader = new VorbisReader(filePath);

            // Convert the NVorbis reader to a wave stream that NAudio can play
            var waveStream = new VorbisWaveReader(vorbisReader);

            // Create a WaveChannel32 to allow volume control
            _waveChannel = new WaveChannel32(waveStream);
            _waveChannel.Volume = _volume; // Set the volume after initializing WaveChannel32

            _waveOutDevice = new WaveOutEvent();
            _waveOutDevice.Init(_waveChannel);
            _waveOutDevice.Play();
        });
    }

    public void StopCurrentSound()
    {
        _waveOutDevice?.Stop();
        _waveChannel?.Dispose();
        _waveOutDevice?.Dispose();
        _waveChannel = null;
        _waveOutDevice = null;
    }
}

// ... (VorbisWaveReader class remains the same)
internal class VorbisWaveReader : WaveStream
{
    private readonly VorbisReader _reader;
    private readonly WaveFormat _waveFormat;

    public VorbisWaveReader(VorbisReader reader)
    {
        _reader = reader;
        _waveFormat = new WaveFormat(_reader.SampleRate, 16, _reader.Channels);
    }

    public override WaveFormat WaveFormat => _waveFormat;
    public override long Length => _reader.TotalSamples * _reader.Channels * 2;

    public override long Position
    {
        get => _reader.DecodedPosition * _reader.Channels * 2;
        set => _reader.DecodedPosition = value / (_reader.Channels * 2);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // Calculate the number of float samples to read.
        // Since we're assuming 16-bit samples, each sample is 2 bytes.
        int samplesToRead = count / 2;

        // Allocate a float array for reading from NVorbis.
        float[] sampleBuffer = new float[samplesToRead];

        // Read the samples.
        int samplesRead = _reader.ReadSamples(sampleBuffer, 0, samplesToRead);

        // Convert the float samples back to 16-bit integers, then to bytes.
        for (int i = 0; i < samplesRead; i++)
        {
            // Convert float sample to 16-bit integer (short).
            short shortSample = (short)(sampleBuffer[i] * short.MaxValue);

            // Place the short value into the byte[] buffer.
            // Note: Be aware of the system's endian-ness. Example assumes little-endian.
            buffer[offset + i * 2] = (byte)(shortSample & 0xFF);
            buffer[offset + i * 2 + 1] = (byte)((shortSample >> 8) & 0xFF);
        }

        // Return the number of bytes written to the buffer.
        return samplesRead * 2; // Since each sample is 2 bytes
    }
}
