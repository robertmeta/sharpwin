using System;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.IO;
using System.Speech.Synthesis;
using NAudio.Wave;
using System.Threading;
using System.Threading.Tasks;

public class AudioTargetQueue
{
    private Queue<PromptBuilder> _textQueue = new Queue<PromptBuilder>();
    private bool _isPlaying = false;
    private CancellationTokenSource _cancellationTokenSource;

    public AudioTargetQueue()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void EnqueueText(PromptBuilder text)
    {
        _textQueue.Enqueue(text);

        if (!_isPlaying)
        {
            var cancellationToken = _cancellationTokenSource.Token;
            PlayQueueAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task PlayQueueAsync(CancellationToken cancellationToken)
    {
        _isPlaying = true;

        while (_textQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            var text = _textQueue.Dequeue();
            await SynthesizeAndPlayAsync(text, cancellationToken);
        }

        _isPlaying = false;
    }

    private Task SynthesizeAndPlayAsync(PromptBuilder text, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        using (var synthesizer = new SpeechSynthesizer())
        using (var ms = new MemoryStream())
        {
            synthesizer.SetOutputToWaveStream(ms);
            synthesizer.Speak(text);

            ms.Position = 0;

            using (var waveOut = new WaveOutEvent())
            using (var rawSource = new RawSourceWaveStream(ms, new WaveFormat(22000, 1)))
            {
                var stereo = new MonoToStereoProvider(rawSource);

                waveOut.Init(stereo);
                waveOut.Play();

                waveOut.PlaybackStopped += (s, e) => tcs.SetResult(true);

                while (waveOut.PlaybackState == PlaybackState.Playing && !cancellationToken.IsCancellationRequested)
                {
                    System.Threading.Thread.Sleep(100); // Reduce CPU usage
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    waveOut.Stop();
                }
            }

            
        }

        if (cancellationToken.IsCancellationRequested)
        {
            tcs.SetCanceled();
        }

        return tcs.Task;
    }

    public void Stop()
    {
        if (_isPlaying)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            // Create a new CancellationTokenSource for future use
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}