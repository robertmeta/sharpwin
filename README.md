# sharpwin

## Introduction 

This is an emacspeak server written in C# intended to be as async as 
reasonable, fast and responsive.

## Installing

1. dotnet build Sharpwin.sln 
2. copy bin/Debug/net8..0/* $EMACSPEAK_DIR/servers
3. add sharpwin to $EMACSPEAK_DIR/servers/.servers

## Emacspeak Setup

1. Get Emacspeak setup as normal, remember, you can run 
it without building compiling the lisp files, just extracting 
it and setting up config is enough. 

If you want to build it properly, hacking the Makefiles up a 
bit, or installing a unix like set of tools, I hope to add a
custom builder to this project soon that will be Make.ps1, 
instructions here will be swapped with how to use Make.ps1 
when I create it.

## Configuration
```
  ; emacspeak paths and such before this 
  (setopt dtk-program "sharpwin")
  ; these are between 0 and 1
  (setenv "SHARPWIN_TONE_VOLUME" "0.1")
  (setenv "SHARPWIN_SOUND_VOLUME" "0.1")
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
