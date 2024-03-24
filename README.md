# sharpwin

## Introduction 

This is an emacspeak server written in swift intended to be as async as 
reasonable, fast and responsive.

Unless you are a developer or interested in becoming one, you probably 
want to use the version bundled with emacspeak, I keep that copy up to 
date with this one fairly consistently. 

## Hacking

The recommended workflow is to symlink the sharpwin binary under either
.build/release/sharpwin or ./build/debug/sharpwin (depending which you are building) to your emacspeak servers directory. 

then just ```make``` for debug or ```make release``` to build a fresh binary.

I will likely remove make install in the future, as it is a bit fidgety, but I 
will leave it in for now. 

## Having Trouble?

### It emits warnings and notes

I am aware of the current warnings, it is a goal to get it to build completely 
clean but tthat is not a priority right now, getting to v2 is the priority.

### Double-Speaking

If you are hearing stuff twice, ensure that mac-ignore-accessibility is set 
and your emacs version supports it. If that doesn't work, you can use the 
VoiceOver Utility that comes with MacOS to create an activity for Emacs.app 
to turn off voiceover while in the Emacs window.  This only works if you are
using a windowed version of Emacs (not terminal version). 
