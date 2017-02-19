# WiredASL
WiredASL is a BlockyTalky program which takes hand gesture inputs and returns musical outputs.

It was designed with deaf and hard of hearing students in mind, and also as an instrument for people with little to no musical experience. The note is determined by a right hand signal, the occidental (flat/sharp/natural) by a left hand signal, and the octave by raising or lowering the left arm. In its current state it works best with two people, one gesturing the note and occidental and the other gesturing the octave.

## Data Flow
WiredASL takes input from two sources: a Leap Motion and a Kinect.

The Leap Motion travels through the following programs:
* [LeapOSC](https://github.com/genekogan/LeapMotionOSC/releases/tag/0.1) for Leap Motion to OSC translation
* [Wekinator](http://wekinator.org) for the machine learning algorithms
* [Processing](https://processing.org) to better control the OSC message
* [BlockyTalky](http://atlas.colorado.edu/lpc/blockyTalky/) on a Raspberry Pi, for musical output

The Kinect goes through these instead:
* Visual Studio to extract raw data
* Processing to bridge Visual Studio and BlockyTalky
* BlockyTalky

## How to Use
TODO
