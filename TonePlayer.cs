using System;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class TonePlayer : IDisposable
{
    private readonly WaveOutEvent waveOut;
    private readonly SignalGenerator signalGenerator;
    private readonly VolumeSampleProvider volumeProvider;

    public TonePlayer()
    {
        waveOut = new WaveOutEvent();
        signalGenerator = new SignalGenerator();
        volumeProvider = new VolumeSampleProvider(signalGenerator);
    }

    public async Task PlayPureToneAsync(int frequencyInHz, int durationInMillis, float volume)
    {
        await Task.Run(() =>
        {
            signalGenerator.Frequency = frequencyInHz;
            signalGenerator.Gain = 0.2f; // Adjust the gain as needed
            signalGenerator.Type = SignalGeneratorType.Sin;

            volumeProvider.Volume = volume;

            waveOut.Init(volumeProvider);
            waveOut.Play();
            Task.Delay(durationInMillis).Wait();
            waveOut.Stop();
        });
    }

    public void Stop()
    {
        waveOut?.Stop();
    }

    public void Dispose()
    {
        waveOut?.Dispose();
    }
}
