using System;
using System.Collections.Concurrent;
using System.Speech.Synthesis;

public class StateStore
{
    private bool _allCapsBeep = false;
    public bool AllCapsBeep
    {
        get { return _allCapsBeep; }
        set { _allCapsBeep = value; }
    }

    private float _characterScale = 1.2f;
    public float CharacterScale
    {
        get { return _characterScale; }
        set { _characterScale = value; }
    }

    private string _audioTarget = "None";
    public string AudioTarget
    {
        get { return _audioTarget.ToLower(); }
        set { _audioTarget = value; }
    }

    private ConcurrentQueue<(string, string)> _pendingQueue = new ConcurrentQueue<(string, string)>();
    public ConcurrentQueue<(string, string)> PendingQueue
    {
        get { return _pendingQueue; }
    }

    public void AppendToPendingQueue((string, string) item)
    {
        _pendingQueue.Enqueue(item);
    }

    public (string cmd, string parameters) PopFromPendingQueue()
    {
        if (_pendingQueue.TryDequeue(out var item))
        {
            return item;
        }
        return (null, null);
    }

    public void ClearPendingQueue()
    {
        _pendingQueue.Clear();
    }

    private float _pitchMultiplier = 1.0f;
    public float PitchMultiplier
    {
        get { return _pitchMultiplier; }
        set { _pitchMultiplier = value; }
    }

    private TimeSpan _postDelay = TimeSpan.Zero;
    public TimeSpan PostDelay
    {
        get { return _postDelay; }
        set { _postDelay = value; }
    }

    private TimeSpan _preDelay = TimeSpan.Zero;
    public TimeSpan PreDelay
    {
        get { return _preDelay; }
        set { _preDelay = value; }
    }

    private string _punctuations = "all";
    public string Punctuations
    {
        get { return _punctuations; }
        set { _punctuations = value; }
    }

    private float _soundVolume = 1f;
    public float SoundVolume
    {
        get { return _soundVolume; }
        set { _soundVolume = value; }
    }

    private int _speechRate = 2;
    public int SpeechRate
    {
        get { return _speechRate; }
        set { _speechRate = value; }
    }

    private bool _splitCaps = false;
    public bool SplitCaps
    {
        get { return _splitCaps; }
        set { _splitCaps = value; }
    }

    private float _toneVolume = 1f;
    public float ToneVolume
    {
        get { return _toneVolume; }
        set { _toneVolume = value; }
    }

    private bool _ttsDiscard = false;
    public bool TtsDiscard
    {
        get { return _ttsDiscard; }
        set { _ttsDiscard = value; }
    }

    private string _voice = "default";
    public string Voice
    {
        get { return _voice; }
        set { _voice = value; }
    }

    private int _voiceVolume = 50;
    public int VoiceVolume
    {
        get { return _voiceVolume; }
        set { _voiceVolume = value; }
    }

    public StateStore()
    {
        SoundVolume = 1.0f;
        if (float.TryParse(GetEnvironmentVariable("SHARPWIN_SOUND_VOLUME"), out float soundVolume))
        {
            SoundVolume = soundVolume;
        }

        ToneVolume = 1.0f;
        if (float.TryParse(GetEnvironmentVariable("SHARPWIN_TONE_VOLUME"), out float toneVolume))
        {
            ToneVolume = toneVolume;
        }

        VoiceVolume = 100;
        if (int.TryParse(GetEnvironmentVariable("SHARPWIN_VOICE_VOLUME"), out int voiceVolume))
        {
            VoiceVolume = voiceVolume;
        }

        AudioTarget = GetEnvironmentVariable("SHARPWIN_AUDIO_TARGET");
    }

    public int GetCharacterRate()
    {
        return (int)Math.Round(SpeechRate * CharacterScale);
    }

    private string GetEnvironmentVariable(string variable)
    {
        return Environment.GetEnvironmentVariable(variable) ?? string.Empty;
    }

    // Setter methods for properties
    public void SetAllCapsBeep(bool value) { _allCapsBeep = value; }
    public void SetCharacterScale(float value) { _characterScale = value; }
    public void SetAudioTarget(string value) { _audioTarget = value; }
    public void SetPitchMultiplier(float value) { _pitchMultiplier = value; }
    public void SetPostDelay(TimeSpan value) { _postDelay = value; }
    public void SetPreDelay(TimeSpan value) { _preDelay = value; }
    public void SetPunctuations(string value) { _punctuations = value; }
    public void SetSoundVolume(float value) { _soundVolume = value; }
    public void SetSpeechRate(int value) { _speechRate = value; }
    public void SetSplitCaps(bool value) { _splitCaps = value; }
    public void SetToneVolume(float value) { _toneVolume = value; }
    public void SetTtsDiscard(bool value) { _ttsDiscard = value; }
    public void SetVoice(string value)
    {
        using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
        {
            string language = null;
            string voiceName = null;

            // Parse the input string to extract language and voice name
            string[] parts = value.Split(':');
            if (parts.Length == 1)
            {
                if (parts[0].StartsWith(":"))
                {
                    voiceName = parts[0].Substring(1);
                }
                else
                {
                    language = parts[0];
                }
            }
            else if (parts.Length == 2)
            {
                language = parts[0];
                voiceName = parts[1];
            }

            // Find the matching voice based on language and voice name
            var matchingVoice = synthesizer.GetInstalledVoices()
                .FirstOrDefault(v =>
                    (string.IsNullOrEmpty(language) || v.VoiceInfo.Culture.Name.StartsWith(language)) &&
                    (string.IsNullOrEmpty(voiceName) || v.VoiceInfo.Name.Contains(voiceName)));

            if (matchingVoice != null)
            {
                _voice = matchingVoice.VoiceInfo.Name;
            }
            else
            {
                _voice = "donotset";
            }
        }

    }
    public void SetVoiceVolume(int value) { _voiceVolume = value; }
}
