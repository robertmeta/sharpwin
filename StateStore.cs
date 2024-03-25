using System;
using System.Collections.Generic;
using System.Linq;

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

    private bool _deadpanMode = false;
    public bool DeadpanMode
    {
        get { return _deadpanMode; }
        set { _deadpanMode = value; }
    }

    private List<(string, string)> _pendingQueue = new List<(string, string)>();
    public List<(string, string)> PendingQueue
    {
        get { return _pendingQueue; }
        set { _pendingQueue = value; }
    }

    public void AppendToPendingQueue((string, string) item)
    {
        _pendingQueue.Add(item);
    }

    public (string cmd, string parameters) PopFromPendingQueue()
    {
        if (_pendingQueue.Any())
        {
            var item = _pendingQueue.First();
            _pendingQueue.RemoveAt(0);
            return item;
        }
        return null;
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

    private float _speechRate = 0.5f;
    public float SpeechRate
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

    private float _voiceVolume = 1f;
    public float VoiceVolume
    {
        get { return _voiceVolume; }
        set { _voiceVolume = value; }
    }

    public StateStore()
    {
        SoundVolume = 1.0f;
        if (float.TryParse(GetEnvironmentVariable("SWIFTMAC_SOUND_VOLUME"), out float soundVolume))
        {
            SoundVolume = soundVolume;
        }

        ToneVolume = 1.0f;
        if (float.TryParse(GetEnvironmentVariable("SWIFTMAC_TONE_VOLUME"), out float toneVolume))
        {
            ToneVolume = toneVolume;
        }

        if (float.TryParse(GetEnvironmentVariable("SWIFTMAC_VOICE_VOLUME"), out float voiceVolume))
        {
            VoiceVolume = voiceVolume;
        }

        if (bool.TryParse(GetEnvironmentVariable("SWIFTMAC_DEADPAN_MODE"), out bool deadpanMode))
        {
            DeadpanMode = deadpanMode;
        }

    }

    public float GetCharacterRate()
    {
        return SpeechRate * CharacterScale;
    }

    private string GetEnvironmentVariable(string variable)
    {
        return Environment.GetEnvironmentVariable(variable) ?? string.Empty;
    }

    // Setter methods for properties
    public void SetAllCapsBeep(bool value) { _allCapsBeep = value; }
    public void SetCharacterScale(float value) { _characterScale = value; }
    public void SetDeadpanMode(bool value) { _deadpanMode = value; }
    public void SetPitchMultiplier(float value) { _pitchMultiplier = value; }
    public void SetPostDelay(TimeSpan value) { _postDelay = value; }
    public void SetPreDelay(TimeSpan value) { _preDelay = value; }
    public void SetPunctuations(string value) { _punctuations = value; }
    public void SetSoundVolume(float value) { _soundVolume = value; }
    public void SetSpeechRate(float value) { _speechRate = value; }
    public void SetSplitCaps(bool value) { _splitCaps = value; }
    public void SetToneVolume(float value) { _toneVolume = value; }
    public void SetTtsDiscard(bool value) { _ttsDiscard = value; }
    public void SetVoice(string value) { _voice = value; }
    public void SetVoiceVolume(float value) { _voiceVolume = value; }
}
