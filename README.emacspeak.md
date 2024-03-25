sharpwin
==============================================================================
This is a drop in replacement for the python "mac" server.

Quick Install (requires C# compiler)
------------------------------------------------------------------------------
 - make sharpwin (from emacspeak root, ignore warnings)
 - Change the server in your configuration to "sharpwin"
 - Restart emacs

Recommended Settings
------------------------------------------------------------------------------
```
  (setopt mac-ignore-accessibility 't)
  (setopt dtk-program "sharpwin")
  ; The below doesn't work but should
  (setopt mac-default-speech-rate 0.5)
  ; Optional volume control
  (setenv "SWIFTMAC_TONE_VOLUME" "0.5")
  (setenv "SWIFTMAC_SOUND_VOLUME" "0.5")
  (setenv "SWIFTMAC_VOICE_VOLUME" "1.0")
  (require 'emacspeak-setup)
  (dtk-set-rate 0.6 t)
```

Dependencies 
------------------------------------------------------------------------------
 - https://github.com/arkasas/OggDecoder.git


Bugs?
------------------------------------------------------------------------------
 - https://github.com/robertmeta/sharpwin/issues
