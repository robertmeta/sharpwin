using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    private static Logger _debugLogger;
    private static string _version = "2.0.0";
    private static string _name = "swiftmac";
    private static StateStore _ss;
    private static SpeechSynthesizer _speaker;
    private static TonePlayer _tonePlayer;

    static async Task Main(string[] args)
    {
        _debugLogger = new Logger("swiftmac-debug.log");
        _ss = new StateStore();
        _speaker = new SpeechSynthesizer();
        _tonePlayer = new TonePlayer();

        _debugLogger.Log("Enter: main");
        await InstantVersion();

        while (true)
        {
            string l = Console.ReadLine();
            _debugLogger.Log($"got line {l}");
            (string cmd, string parameters) = await IsolateCmdAndParams(l);
            switch (cmd)
            {
                case "a":
                    await ProcessAndQueueAudioIcon(parameters);
                    break;
                case "c":
                    await ProcessAndQueueCodes(l);
                    break;
                case "d":
                    await DispatchPendingQueue();
                    break;
                case "l":
                    await InstantLetter(parameters);
                    break;
                case "p":
                    await DoPlaySound(parameters);
                    break;
                case "q":
                    await ProcessAndQueueSpeech(parameters);
                    break;
                case "s":
                    await QueueLine(cmd, parameters);
                    break;
                case "sh":
                    await QueueLine(cmd, parameters);
                    break;
                case "t":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_allcaps_beep":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_exit":
                    await InstantTtsExit();
                    break;
                case "tts_pause":
                    await InstantTtsPause();
                    break;
                case "tts_reset":
                    await InstantTtsReset();
                    break;
                case "tts_resume":
                    await InstantTtsResume();
                    break;
                case "tts_say":
                    await InstantTtsSay(parameters);
                    break;
                case "tts_set_character_scale":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_set_pitch_multiplier":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_set_punctuations":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_set_sound_volume":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_set_speech_rate":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_set_tone_volume":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_set_voice":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_set_voice_volume":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_split_caps":
                    await QueueLine(cmd, parameters);
                    break;
                case "tts_sync_state":
                    await ProcessAndQueueSync(l);
                    break;
                case "version":
                    await InstantVersion();
                    break;
                default:
                    await UnknownLine(l);
                    break;
            }
        }
    }

    private static async Task DispatchPendingQueue()
    {
        while (_ss.PopFromPendingQueue() is (string cmd, string parameters) item)
        {
            _debugLogger.Log($"got queued {item.cmd} {item.parameters}");
            switch (item.cmd)
            {
                case "p":
                    await DoPlaySound(item.parameters);
                    break;
                case "s":
                    await DoStopSpeaking();
                    break;
                case "sh":
                    await DoSilence(item.parameters);
                    break;
                case "speak":
                    await DoSpeak(item.parameters);
                    break;
                case "t":
                    await DoTone(item.parameters);
                    break;
                case "tts_allcaps_beep":
                    await TtsAllCapsBeep(item.parameters);
                    break;
                case "tts_set_character_scale":
                    await TtsSetCharacterScale(item.parameters);
                    break;
                case "tts_set_pitch_multiplier":
                    await TtsSetPitchMultiplier(item.parameters);
                    break;
                case "tts_set_punctuations":
                    await TtsSetPunctuations(item.parameters);
                    break;
                case "tts_set_sound_volume":
                    await TtsSetSoundVolume(item.parameters);
                    break;
                case "tts_set_speech_rate":
                    await TtsSetSpeechRate(item.parameters);
                    break;
                case "tts_set_tone_volume":
                    await TtsSetToneVolume(item.parameters);
                    break;
                case "tts_set_voice":
                    await TtsSetVoice(item.parameters);
                    break;
                case "tts_set_voice_volume":
                    await TtsSetVoiceVolume(item.parameters);
                    break;
                case "tts_split_caps":
                    await TtsSplitCaps(item.parameters);
                    break;
                default:
                    await ImpossibleQueue(item.cmd, item.parameters);
                    break;
            }
        }
    }

    private static async Task QueueLine(string cmd, string parameters)
    {
        _debugLogger.Log("Enter: queueLine");
        _ss.AppendToPendingQueue((cmd, parameters));
    }

    private static async Task<List<string>> SplitOnSquareStar(string input)
    {
        string separator = "[*]";
        List<string> result = new List<string>();

        string[] parts = input.Split(new[] { separator }, StringSplitOptions.None);
        for (int index = 0; index < parts.Length; index++)
        {
            result.Add(parts[index]);
            // Add the separator back except after the last part
            if (index < parts.Length - 1)
            {
                result.Add(separator);
            }
        }

        return result;
    }

    private static async Task ProcessAndQueueSpeech(string p)
    {
        string temp;
        if (_ss.SplitCaps)
        {
            temp = InsertSpaceBeforeUppercase(p);
        }
        else
        {
            temp = p;
        }

        List<string> parts = await SplitOnSquareStar(temp);
        foreach (string part in parts)
        {
            if (part == "[*]")
            {
                _ss.AppendToPendingQueue(("sh", "0"));
            }
            else
            {
                string speakPart = await ReplacePunctuations(temp);
                _ss.AppendToPendingQueue(("speak", speakPart));
            }
        }
    }

    private static string InsertSpaceBeforeUppercase(string input)
    {
        _debugLogger.Log("Enter: insertSpaceBeforeUppercase");
        string pattern = "(?<=[a-z])(?=[A-Z])";
        Regex regex = new Regex(pattern);
        string modifiedString = regex.Replace(input, " ");
        return modifiedString;
    }

    private static async Task InstantTtsReset()
    {
        _debugLogger.Log("Enter: instantTtsReset");
        await DoStopAll();
        _ss = new StateStore();
    }

    private static async Task InstantVersion()
    {
        _debugLogger.Log("Enter: instantVersion");
        string sayVersion = _version.Replace(".", " dot ");

        await DoStopAll();
        await InstantTtsSay($"{_name} {sayVersion}");
    }

    private static async Task DoSilence(string p)
    {
        _debugLogger.Log("Enter: doSilence");
        TimeSpan oldPostDelay = _ss.PostDelay;
        if (int.TryParse(p, out int durationInMillis))
        {
            _ss.SetPostDelay(TimeSpan.FromMilliseconds(durationInMillis));
        }
        await DoSpeak("");
        _ss.SetPostDelay(oldPostDelay);
    }

    private static async Task InstantTtsResume()
    {
        _debugLogger.Log("Enter: instantTtsResume");
        _speaker.Resume();
    }

    private static async Task InstantLetter(string p)
    {
        _debugLogger.Log("Enter: unknownLine");
        float oldPitchMultiplier = _ss.PitchMultiplier;
        TimeSpan oldPreDelay = _ss.PreDelay;
        if (IsFirstLetterCapital(p))
        {
            if (_ss.AllCapsBeep)
            {
                await DoTone("500 50");
            }
            else
            {
                _ss.SetPitchMultiplier(1.5f);
            }
        }
        float oldSpeechRate = _ss.SpeechRate;
        _ss.SetSpeechRate(_ss.GetCharacterRate());
        await DoStopSpeaking();
        await DoSpeak(p.ToLower());
        _ss.SetPitchMultiplier(oldPitchMultiplier);
        _ss.SetSpeechRate(oldSpeechRate);
        _ss.SetPreDelay(oldPreDelay);
    }

    private static async Task DoStopSpeaking()
    {
        _debugLogger.Log("Enter: doStopSpeaking");
        _speaker.SpeakAsyncCancelAll();
    }

    private static bool IsFirstLetterCapital(string str)
    {
        _debugLogger.Log("Enter: isFirstLetterCapital");
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        char firstChar = str[0];
        return char.IsUpper(firstChar) && char.IsLetter(firstChar);
    }

    private static async Task InstantTtsPause()
    {
        _debugLogger.Log("Enter: instantTtsPause");
        _speaker.Pause();
    }

    private static async Task UnknownLine(string line)
    {
        _debugLogger.Log("Enter: unknownLine");
        _debugLogger.Log($"Unknown command: {line}");
        Console.WriteLine($"Unknown command: {line}");
    }

    private static async Task ImpossibleQueue(string cmd, string parameters)
    {
        _debugLogger.Log("Enter: impossibleQueue");
        _debugLogger.Log($"Impossible queue item '{cmd}' '{parameters}'");
        Console.WriteLine($"Impossible queue item '{cmd}' '{parameters}'");
    }

    private static string ExtractVoice(string str)
    {
        _debugLogger.Log("Enter: extractVoice");
        string pattern = @"\[\{voice\s+([^\}]+)\}\]";
        Match match = Regex.Match(str, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }

    private static async Task ProcessAndQueueAudioIcon(string p)
    {
        _debugLogger.Log("Enter: processAndQueueAudioIcon");
        _ss.AppendToPendingQueue(("p", p));
    }

    private static async Task ProcessAndQueueCodes(string p)
    {
        _debugLogger.Log("Enter: processAndQueueCodes");
        string voice = ExtractVoice(p);
        if (!string.IsNullOrEmpty(voice))
        {
            _ss.AppendToPendingQueue(("tts_set_voice", voice));
        }
    }

    private static async Task<string> ReplacePunctuations(string s)
    {
        switch (_ss.Punctuations)
        {
            case "all":
                return ReplaceAllPuncs(s);
            case "some":
                return ReplaceSomePuncs(s);
            default:
                return ReplaceBasePuncs(s);
        }
    }

    private static string ReplaceBasePuncs(string line)
    {
        _debugLogger.Log("Enter: replaceBasePuncs");
        return line
            .Replace("%", " percent ")
            .Replace("$", " dollar ");
    }

    private static string ReplaceSomePuncs(string line)
    {
        _debugLogger.Log("Enter: replaceSomePuncs");
        return ReplaceBasePuncs(line)
            .Replace("#", " pound ")
            .Replace("-", " dash ")
            .Replace("\"", " quote ")
            .Replace("(", " leftParen ")
            .Replace(")", " rightParen ")
            .Replace("*", " star ")
            .Replace(";", " semi ")
            .Replace(":", " colon ")
            .Replace("\n", "")
            .Replace("\\", " backslash ")
            .Replace("/", " slash ")
            .Replace("+", " plus ")
            .Replace("=", " equals ")
            .Replace("~", " tilda ")
            .Replace("`", " backquote ")
            .Replace("!", " exclamation ")
            .Replace("^", " caret ");
    }

    private static string ReplaceAllPuncs(string line)
    {
        _debugLogger.Log("Enter: replaceAllPuncs");
        return ReplaceSomePuncs(line)
            .Replace("<", " less than ")
            .Replace(">", " greater than ")
            .Replace("'", " apostrophe ")
            .Replace("*", " star ")
            .Replace("@", " at sign ")
            .Replace("_", " underline ")
            .Replace(".", " dot ")
            .Replace(",", " comma ");
    }

    private static async Task TtsSplitCaps(string p)
    {
        _debugLogger.Log("Enter: ttsSplitCaps");
        _ss.SetSplitCaps(p == "1");
    }

    private static async Task TtsSetVoice(string p)
    {
        _debugLogger.Log("Enter: ttsSetVoice");
        _ss.SetVoice(p);
    }

    private static async Task TtsSetToneVolume(string p)
    {
        _debugLogger.Log("Enter: ttsSetToneVolume");
        if (float.TryParse(p, out float toneVolume))
        {
            _ss.SetToneVolume(toneVolume);
        }
    }

    private static async Task TtsSetSoundVolume(string p)
    {
        _debugLogger.Log("Enter: ttsSetSoundVolume");
        if (float.TryParse(p, out float soundVolume))
        {
            _ss.SetSoundVolume(soundVolume);
        }
    }

    private static async Task TtsSetVoiceVolume(string p)
    {
        _debugLogger.Log("Enter: ttsSetVoiceVolume");
        if (float.TryParse(p, out float voiceVolume))
        {
            _ss.SetVoiceVolume(voiceVolume);
        }
    }

    private static async Task TtsSetSpeechRate(string p)
{
_debugLogger.Log("Enter: ttsSetSpeechRate");
if (float.TryParse(p, out float speechRate))
{
_ss.SetSpeechRate(speechRate);
}
}


Copy code
private static async Task TtsSetPitchMultiplier(string p)
{
    _debugLogger.Log("Enter: ttsSetPitchMultiplier");
    if (float.TryParse(p, out float pitchMultiplier))
    {
        _ss.SetPitchMultiplier(pitchMultiplier);
    }
}

private static async Task TtsSetPunctuations(string p)
{
    _debugLogger.Log("Enter: ttsSetPunctuations");
    _ss.SetPunctuations(p);
}

private static async Task TtsSetCharacterScale(string p)
{
    _debugLogger.Log("Enter: ttsSetCharacterScale");
    if (float.TryParse(p, out float characterScale))
    {
        _ss.SetCharacterScale(characterScale);
    }
}

private static async Task TtsAllCapsBeep(string p)
{
    _debugLogger.Log("Enter: ttsAllCapsBeep");
    _ss.SetAllCapsBeep(p == "1");
}

private static async Task ProcessAndQueueSync(string p)
{
    _debugLogger.Log("Enter: processAndQueueSync");
    string[] ps = p.Split(' ');
    if (ps.Length == 4)
    {
        string punct = ps[0];
        _ss.AppendToPendingQueue(("tts_set_punctuations", punct));

        string splitCaps = ps[1];
        _ss.AppendToPendingQueue(("tts_split_caps", splitCaps));

        string beepCaps = ps[2];
        _ss.AppendToPendingQueue(("tts_allcaps_beep", beepCaps));

        string rate = ps[3];
        _ss.AppendToPendingQueue(("tts_set_speech_rate", rate));
    }
}

private static async Task DoTone(string p)
{
    _debugLogger.Log("Enter: doTone");
    string[] ps = p.Split(' ');
    int frequency = int.TryParse(ps[0], out int f) ? f : 500;
    int durationInMillis = int.TryParse(ps[1], out int d) ? d : 75;

    await _tonePlayer.PlayPureToneAsync(frequency, durationInMillis);
}

private static async Task DoPlaySound(string p)
{
    _debugLogger.Log("Enter: doPlaySound");
    await SoundManager.Instance.PlaySoundAsync(p, _ss.SoundVolume);
}

private static async Task InstantTtsSay(string p)
{
    _debugLogger.Log("Enter: instantTtsSay");
    _debugLogger.Log($"ttsSay: {p}");
    await DoStopAll();
    await DoSpeak(p);
}

private static async Task DoStopAll()
{
    _debugLogger.Log("Enter: doStopAll");
    await DoStopSpeaking();
    await _tonePlayer.StopAsync();
    SoundManager.Instance.StopCurrentSound();
}

private static async Task DoSpeak(string what)
{
    List<string> parts = await SplitOnSquareStar(what);
    foreach (string part in parts)
    {
        if (part == "[*]")
        {
            await DoSilence("0");
        }
        else
        {
            await _DoSpeak(part);
        }
    }
}

private static async Task _DoSpeak(string what)
{
    _debugLogger.Log("Enter: doSpeak");
    PromptBuilder builder = new PromptBuilder();
    builder.AppendText(what);

    // Set the rate of speech (0.5 to 1.0)
    _speaker.Rate = _ss.SpeechRate;

    // Set the pitch (0.5 to 2.0)
    _speaker.Pitch = _ss.PitchMultiplier;

    // Set the volume (0.0 to 1.0)
    _speaker.Volume = _ss.VoiceVolume;

    // Set the voice
    _speaker.SelectVoice(_ss.Voice);

    // Start speaking
    _speaker.SpeakAsync(builder);
}

private static async Task InstantTtsExit()
{
    _debugLogger.Log("Enter: instantTtsExit");
    Environment.Exit(0);
}

private static async Task<string> IsolateCommand(string line)
{
    _debugLogger.Log("Enter: isolateCommand");
    string cmd = line.Trim();
    int firstIndex = cmd.IndexOf(' ');
    if (firstIndex >= 0)
    {
        cmd = cmd.Substring(0, firstIndex);
    }
    return cmd;
}

private static async Task<(string, string)> IsolateCmdAndParams(string line)
{
    _debugLogger.Log("Enter: isolateParams");
    string justCmd = await IsolateCommand(line);
    string cmd = justCmd + " ";

    string parameters = Regex.Replace(line, "^" + Regex.Escape(cmd), "", RegexOptions.IgnoreCase);
    parameters = parameters.Trim();
    if (parameters.StartsWith("{") && parameters.EndsWith("}"))
    {
        parameters = parameters.Substring(1, parameters.Length - 2);
    }
    _debugLogger.Log($"Exit: isolateParams: {parameters}");
    return (justCmd, parameters);
}
}
