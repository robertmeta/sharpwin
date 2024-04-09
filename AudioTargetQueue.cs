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
    private Queue<string> _textQueue = new Queue<string>();
    private bool _isPlaying = false;
    private CancellationTokenSource _cancellationTokenSource;
    private string _target = "None";

    public AudioTargetQueue()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void SetTarget(string t)
    {
        _target = t;
    }

    public void EnqueueText(string text)
    {
        _textQueue.Enqueue(text);
        if (!_isPlaying)
        {
            var cancellationToken = _cancellationTokenSource.Token;
            _ = PlayQueueAsync(cancellationToken);
        }
    }




    public void Stop()
    {
        _textQueue.Clear();
        if (_isPlaying)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }

    private async Task PlayQueueAsync(CancellationToken cancellationToken)
    {
        _isPlaying = true;
        while (_textQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            var text = _textQueue.Dequeue();
            try
            {
                await SynthesizeAndPlayAsync(text, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully
                break;
            }
        }
        _isPlaying = false;
    }






    private async Task SynthesizeAndPlayAsync(string text, CancellationToken cancellationToken)
    {
        using (var ms = new MemoryStream())
        {
            using (var synthesizer = new SpeechSynthesizer())
            {
                synthesizer.SetOutputToWaveStream(ms);
                synthesizer.SpeakSsml(text);
            }

            ms.Position = 0;

            using (var waveOut = new WaveOutEvent())
            using (var rawSource = new RawSourceWaveStream(ms, new WaveFormat(22000, 1)))
            {
                var stereo = new MonoToStereoProvider(rawSource, _target);
                waveOut.Init(stereo);
                waveOut.Play();

                while (waveOut.PlaybackState == PlaybackState.Playing && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(100, cancellationToken);
                }

                waveOut.Stop();
            }
        }
    }

}
