# sharpwin

## Introduction 

This is an emacspeak server written in C# intended to be as async as 
reasonable, fast and responsive.

## Installing

1. Switch to Sharpwin directory in powershell
2. $env:EMACSPEAK_DIR="C:\Path\To\Emacspeak"
3. ./make.ps1
4. Setup your init.el with normal emacspeak stuff, example below

## Configuration

```
  ; (optional) these are between 0 and 1
  (setenv "SHARPWIN_TONE_VOLUME" "1.0")
  (setenv "SHARPWIN_SOUND_VOLUME" "1.0")

  ; (optional) this is between 0 and 100
  (setenv "SHARPWIN_VOICE_VOLUME" "100")

  ; required parts
  (add-to-list 'load-path "path/to/emacspeak/lisp")
  (setopt dtk-program "sharpwin")
  (setopt tts-notification-device "right")
  (require 'emacspeak-setup)
  (push "sharpwin" tts-multi-engines)
  ; Heree you can just do "en-US" or just ":Zira"
  (dtk-set-language "en-US:Zira")
  (dtk-set-rate 8 t)
```

## Currently Missing

- Pitch Control
