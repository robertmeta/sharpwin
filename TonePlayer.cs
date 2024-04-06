using System;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class TonePlayer : IDisposable
{
    private readonly WaveOutEvent waveOut;
    private readonly SignalGenerator signalGenerator;

    public TonePlayer()
    {
        waveOut = new WaveOutEvent();
        signalGenerator = new SignalGenerator();
    }

    public async Task PlayPureToneAsync(int frequencyInHz, int durationInMillis, float volume)
    {
        await Task.Run(() =>
        {
            signalGenerator.Frequency = frequencyInHz;
            signalGenerator.Gain = volume;
            signalGenerator.Type = SignalGeneratorType.Sin;

            waveOut.Init(signalGenerator);
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
