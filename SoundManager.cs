using NAudio.Wave;
using NVorbis;
using System;
using System.IO;
using System.Threading.Tasks;

public class SoundManager
{
    private static readonly Lazy<SoundManager> _instance = new Lazy<SoundManager>(() => new SoundManager());
    public static SoundManager Instance => _instance.Value;

    private IWavePlayer _waveOutDevice;
    private WaveStream _waveStream;

    private SoundManager()
    {
        // Private constructor to enforce singleton pattern
    }

    public async Task PlaySoundAsync(string filePath)
    {
        StopCurrentSound(); // Stop and clean up the previous sound if any

        // NVorbis supports Vorbis format. For other formats, consider using NAudio's built-in decoders.
        await Task.Run(() =>
        {
            var vorbisReader = new VorbisWaveReader(filePath); // NVorbis to read the Ogg Vorbis file
            _waveOutDevice = new WaveOutEvent(); // NAudio's WaveOutEvent for playback
            _waveStream = vorbisReader; // Directly use VorbisWaveReader as a WaveStream

            _waveOutDevice.Init(_waveStream);
        });

        _waveOutDevice.Play();
    }

    public void StopCurrentSound()
    {
        if (_waveOutDevice != null)
        {
            _waveOutDevice.Stop();
            _waveOutDevice.Dispose();
            _waveOutDevice = null;
        }

        if (_waveStream != null)
        {
            _waveStream.Dispose();
            _waveStream = null;
        }
    }
}
