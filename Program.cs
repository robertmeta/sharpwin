using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Speech.Synthesis;
using NAudio.Wave;

class Program
{
    private static Logger _debugLogger = Logger.GetInstance();
    private static string _version = "1.1.2";
    private static string _name = "SharpWin";
    private static StateStore _ss = new StateStore();
    private static SpeechSynthesizer _speaker = new SpeechSynthesizer();
    private static TonePlayer _tonePlayer = new TonePlayer();
    private static AudioTargetQueue _audioQueue = new AudioTargetQueue();

    static async Task Main(string[] args)
    {
        try
        {
            await _debugLogger.Log("Enter: main");
            await InstantVersion();

            _audioQueue.SetTarget(_ss.AudioTarget);

            while (true)
            {
                try
                {
                    string l = Console.ReadLine();
                    await _debugLogger.Log($"got line {l}");
                    (string cmd, string parameters) = await IsolateCmdAndParams(l);
                    switch (cmd)
                    {
                        case "a":
                            await ProcessAndQueueAudioIcon(parameters);
                            break;
                        case "c":
                            await DoDiscard(cmd, parameters);
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
                            await QueueLine(cmd, parameters);
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
                            await DoDiscard(cmd, parameters);
                            //await InstantTtsPause();
                            break;
                        case "tts_reset":
                            await InstantTtsReset();
                            break;
                        case "tts_resume":
                            await DoDiscard(cmd, parameters);
                            //await InstantTtsResume();
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
                        case "set_lang":
                            await TtsSetVoice(parameters);
                            break;
                        case "tts_set_voice_volume":
                            await QueueLine(cmd, parameters);
                            break;
                        case "tts_split_caps":
                            await QueueLine(cmd, parameters);
                            break;
                        case "tts_sync_state":
                            await DoDiscard(cmd, parameters);
                            //await ProcessAndQueueSync(l);
                            break;
                        case "version":
                            await InstantVersion();
                            break;
                        default:
                            await UnknownLine(l);
                            break;
                    }
                }
                catch (Exception e)
                {
                    _debugLogger.Log($"Inner Error: {e.Message}");
                    Console.WriteLine($"Error: {e.Message}");
                }
            } // End of while loop
        }
        catch (Exception e)
        {
            _debugLogger.Log($"Outer Error: {e.Message}");
            Console.WriteLine($"Error: {e.Message}");
        }

    }

    private static async Task DispatchPendingQueue()
    {
        (string cmd, string parameters) item;
        while ((item = _ss.PopFromPendingQueue()) != (null, null))
        {
            await _debugLogger.Log($"got queued {item.cmd} {item.parameters}");
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
                case "q":
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
        await _debugLogger.Log("Enter: queueLine");
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
    }

    private static async Task<string> InsertSpaceBeforeUppercase(string input)
    {
        await _debugLogger.Log("Enter: insertSpaceBeforeUppercase");
        string pattern = "(?<=[a-z])(?=[A-Z])";
        Regex regex = new Regex(pattern);
        string modifiedString = regex.Replace(input, " ");
        return modifiedString;
    }

    private static async Task InstantTtsReset()
    {
        await _debugLogger.Log("Enter: instantTtsReset");
        await DoStopAll();
        _ss = new StateStore();
    }

    private static async Task InstantVersion()
    {
        await _debugLogger.Log("Enter: instantVersion");
        string sayVersion = _version.Replace(".", " dot ");

        await DoStopAll();
        await InstantTtsSay($"{_name} {sayVersion}");
    }

    private static async Task DoSilence(string p)
    {
        await _debugLogger.Log("Enter: doSilence");
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
        await _debugLogger.Log("Enter: instantTtsResume");
        _speaker.Resume();
    }

    private static async Task InstantLetter(string p)
    {
        await _debugLogger.Log("Enter: unknownLine");
        float oldPitchMultiplier = _ss.PitchMultiplier;
        TimeSpan oldPreDelay = _ss.PreDelay;
        if (await IsFirstLetterCapital(p))
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
        int oldSpeechRate = _ss.SpeechRate;
        _ss.SetSpeechRate(_ss.GetCharacterRate());
        await DoStopSpeaking();
        await DoSpeak(p.ToLower());
        _ss.SetPitchMultiplier(oldPitchMultiplier);
        _ss.SetSpeechRate(oldSpeechRate);
        _ss.SetPreDelay(oldPreDelay);
    }

    private static async Task DoDiscard(string c, string p)
    {
        await _debugLogger.Log($"Intentionally Discarded: {c} {p}");
    }


    private static async Task DoStopSpeaking()
    {
        await _debugLogger.Log("Enter: doStopSpeaking");
        _speaker.SpeakAsyncCancelAll();
        _audioQueue.Stop();
    }

    private static async Task<bool> IsFirstLetterCapital(string str)
    {
        await _debugLogger.Log("Enter: isFirstLetterCapital");
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        char firstChar = str[0];
        return char.IsUpper(firstChar) && char.IsLetter(firstChar);
    }

    private static async Task InstantTtsPause()
    {
        await _debugLogger.Log("Enter: instantTtsPause");
        _speaker.Pause();
    }

    private static async Task UnknownLine(string line)
    {
        await _debugLogger.Log("Enter: unknownLine");
        await _debugLogger.Log($"Unknown command: {line}");
        Console.WriteLine($"Unknown command: {line}");
    }

    private static async Task ImpossibleQueue(string cmd, string parameters)
    {
        await _debugLogger.Log("Enter: impossibleQueue");
        await _debugLogger.Log($"Impossible queue item '{cmd}' '{parameters}'");
        Console.WriteLine($"Impossible queue item '{cmd}' '{parameters}'");
    }

    private static async Task<string> ExtractVoice(string str)
    {
        await _debugLogger.Log("Enter: extractVoice");
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
        await _debugLogger.Log("Enter: processAndQueueAudioIcon");
        _ss.AppendToPendingQueue(("p", p));
    }

    private static async Task ProcessAndQueueCodes(string p)
    {
        await _debugLogger.Log("Enter: processAndQueueCodes");
        string voice = await ExtractVoice(p);
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
                return (await ReplaceAllPuncs(s));
            case "some":
                return (await ReplaceSomePuncs(s));
            default:
                return (await ReplaceBasePuncs(s));
        }
    }

    private static async Task<string> ReplaceBasePuncs(string line)
    {
        await _debugLogger.Log("Enter: replaceBasePuncs");
        return line
            .Replace("%", " percent ")
            .Replace("$", " dollar ");
    }

    private static async Task<string> ReplaceSomePuncs(string line)
    {
        await _debugLogger.Log("Enter: replaceSomePuncs");
        return (await ReplaceBasePuncs(line))
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

    private static async Task<string> ReplaceAllPuncs(string line)
    {
        await _debugLogger.Log("Enter: replaceAllPuncs");
        return (await ReplaceSomePuncs(line))
            .Replace("<", " less than ")
            .Replace(">", " greater than ")
            .Replace("'", " apostrophe ")
            .Replace("*", " star ")
            .Replace("@", " at sign ")
            .Replace("_", " underline ")
            .Replace(".", " dot ")
            .Replace(",", " comma ");
    }

    private static async Task<string> BuildSsml(string p)
    {
        await _debugLogger.Log("Enter: BuildSsml");

        // <audio src='path/to/your/audio/file.wav'/>
        string ssml = @"
<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
    <voice name='" + _ss.Voice + @"'>
            " + p + @"
    </voice>
</speak>";
        return ssml;
    }


    private static async Task TtsSplitCaps(string p)
    {
        await _debugLogger.Log("Enter: TtsSplitCaps");
        _ss.SetSplitCaps(p == "1");
    }

    private static async Task TtsSetVoice(string p)
    {
        await _debugLogger.Log("Enter: ttsSetVoice");
        string[] ps = p.Split(' ');
        if (ps.Length == 2)
        {
            _ss.SetVoice(ps[0]);
            if (ps[1] == "t")
            {
                DoSpeak("Voice set to: " + _ss.Voice);
            }
        }
    }

    private static async Task TtsSetToneVolume(string p)
    {
        await _debugLogger.Log("Enter: ttsSetToneVolume");
        if (float.TryParse(p, out float toneVolume))
        {
            _ss.SetToneVolume(toneVolume);
        }
    }

    private static async Task TtsSetSoundVolume(string p)
    {
        await _debugLogger.Log("Enter: ttsSetSoundVolume");
        if (float.TryParse(p, out float soundVolume))
        {
            _ss.SetSoundVolume(soundVolume);
        }
    }

    private static async Task TtsSetVoiceVolume(string p)
    {
        await _debugLogger.Log("Enter: ttsSetVoiceVolume");
        if (int.TryParse(p, out int voiceVolume))
        {
            _ss.SetVoiceVolume(voiceVolume);
        }
    }

    private static async Task TtsSetSpeechRate(string p)
    {
        await _debugLogger.Log("Enter: ttsSetSpeechRate");
        if (int.TryParse(p, out int speechRate))
        {
            _ss.SetSpeechRate(speechRate);
        }
    }



    private static async Task TtsSetPitchMultiplier(string p)
    {
        await _debugLogger.Log("Enter: ttsSetPitchMultiplier");
        if (float.TryParse(p, out float pitchMultiplier))
        {
            _ss.SetPitchMultiplier(pitchMultiplier);
        }
    }

    private static async Task TtsSetPunctuations(string p)
    {
        await _debugLogger.Log("Enter: ttsSetPunctuations");
        _ss.SetPunctuations(p);
    }

    private static async Task TtsSetCharacterScale(string p)
    {
        await _debugLogger.Log("Enter: ttsSetCharacterScale");
        if (float.TryParse(p, out float characterScale))
        {
            _ss.SetCharacterScale(characterScale);
        }
    }

    private static async Task TtsAllCapsBeep(string p)
    {
        await _debugLogger.Log("Enter: ttsAllCapsBeep");
        _ss.SetAllCapsBeep(p == "1");
    }

    private static async Task ProcessAndQueueSync(string p)
    {
        await _debugLogger.Log("Enter: processAndQueueSync");
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
        await _debugLogger.Log("Enter: doTone");
        string[] ps = p.Split(' ');
        int frequency = int.TryParse(ps[0], out int f) ? f : 500;
        int durationInMillis = int.TryParse(ps[1], out int d) ? d : 75;

        await _tonePlayer.PlayPureToneAsync(frequency, durationInMillis, _ss.ToneVolume);
    }

    private static async Task DoPlaySound(string p)
    {
        await _debugLogger.Log("Enter: doPlaySound");
        SoundManager.Instance.Volume = _ss.SoundVolume;
        await SoundManager.Instance.PlaySoundAsync(p);
    }

    private static async Task InstantTtsSay(string p)
    {
        await _debugLogger.Log("Enter: instantTtsSay");
        await _debugLogger.Log($"ttsSay: {p}");
        await DoStopAll();
        await DoSpeak(p);
    }

    private static async Task DoStopAll()
    {
        await _debugLogger.Log("Enter: doStopAll");
        await DoStopSpeaking();
        _tonePlayer.Stop();
        SoundManager.Instance.StopCurrentSound();
    }

    private static async Task DoSpeak(string what)
    {
        string temp;
        if (_ss.SplitCaps)
        {
            temp = await InsertSpaceBeforeUppercase(what);
        }
        else
        {
            temp = what;
        }

        List<string> parts = await SplitOnSquareStar(temp);
        foreach (string part in parts)
        {
            if (part == "[*]")
            {
                await DoSilence("0");
            }
            else
            {
                string speakPart = await ReplacePunctuations(temp);
                await _DoSpeak(speakPart);
            }
        }

    }

    private static async Task _DoSpeak(string what)
    {
        await _debugLogger.Log("Enter: doSpeak");
        // Important: lots of the settings live in
        // this builder
        string ssml = await BuildSsml(what);

        // Set the rate of speech (-19 to 10)
        _speaker.Rate = _ss.SpeechRate;


        // Set the volume (0 to 100)
        _speaker.Volume = _ss.VoiceVolume;

        // Start speaking
        await _debugLogger.Log($"SSML: {ssml}");
        if (_ss.AudioTarget == "right" || _ss.AudioTarget == "left")
        {
            await _debugLogger.Log($"EnqueueText");
            _audioQueue.EnqueueText(ssml);
        }
        else
        {
            await _debugLogger.Log($"SpeakSsmlasync");
            _speaker.SpeakSsmlAsync(ssml);
        }

    }

    private static async Task InstantTtsExit()
    {
        await _debugLogger.Log("Enter: instantTtsExit");
        Environment.Exit(0);
    }

    private static async Task<string> IsolateCommand(string line)
    {
        await _debugLogger.Log("Enter: isolateCommand");
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
        await _debugLogger.Log("Enter: isolateParams");
        string justCmd = await IsolateCommand(line);
        string cmd = justCmd + " ";

        string parameters = Regex.Replace(line, "^" + Regex.Escape(cmd), "", RegexOptions.IgnoreCase);
        parameters = parameters.Trim();
        if (parameters.StartsWith("{") && parameters.EndsWith("}"))
        {
            parameters = parameters.Substring(1, parameters.Length - 2);
        }
        await _debugLogger.Log($"Exit: isolateParams: {parameters}");
        return (justCmd, parameters);
    }
}
