# sharpwin

## Requirements

- Git Bash (installs with Git for Windows)
-- Available from https://git-scm.com/download/win
- Dotnet 8.0+
-- https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Introduction 

This is an emacspeak server written in C# intended to be as async as 
reasonable, fast and responsive.

It is assumed if you are using this from this repo, you are also using 
the git version of emacspeak, this tracks very close to the ongoing 
emacspeak, and some requirements for this may well live in the git repo
version of emacspeak. 

## Installing

1. Open Git Bash (or your favorite bash)
2. ```make install```
3. Setup your init.el as shown below

## Configuration

```
  ; (optional) these are between 0 and 1
  (setenv "SHARPWIN_TONE_VOLUME" "1.0")
  (setenv "SHARPWIN_SOUND_VOLUME" "1.0")

  ; (optional) this is between 0 and 100
  (setenv "SHARPWIN_VOICE_VOLUME" "100")

  ; (required) parts
  (setopt dtk-program "sharpwin")
  (setopt tts-notification-device "right")
  (add-to-list 'load-path "path/to/emacspeak/lisp")
  (require 'emacspeak-setup)

  ; (optional) Setup voice and rate if you want
  (dtk-set-language "en-US:Zira")
  (dtk-set-rate 8 t)
```

## Currently Missing

- Pitch Control
