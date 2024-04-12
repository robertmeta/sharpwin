# sharpwin

## Introduction 

This is an emacspeak server written in C# intended to be as async as 
reasonable, fast and responsive.

## Installing

1. Switch to Sharpwin directory in powershell
2. $env:EMACSPEAK_DIR="/path/to/emacspeak"
3. ./make.ps1
4. Setup your init.el with normal emacspeak stuff, example below

## Configuration
```
  (add-to-list 'load-path "path/to/emacspeak/lisp")
  (setopt dtk-program "sharpwin")
  ; these are between 0 and 1
  (setenv "SHARPWIN_TONE_VOLUME" "1.0")
  (setenv "SHARPWIN_SOUND_VOLUME" "1.0")
  ; this is between 0 and 100
  (setenv "SHARPWIN_VOICE_VOLUME" "100")
  (push "sharpwin" tts-multi-engines)
  (setopt tts-notification-device "right")
  (require 'emacspeak-setup)
  ; Heree you can just do "en-US" or just ":Zira"
  (dtk-set-language "en-US:Zira")
  (dtk-set-rate 8 t)
```

## Currently Broken

1. No log-sharpwin verison yet, I might make this a flag and do the logging
directly from SharpWin, I miss Tee. 
2. Rate control seems a bit goofy on notifications side, this might be a bug 
in emacspeak
3. I am sure lots more, this is raw. 
