# CutLang
 Language describing how to cut up videos. Capable of very basic video editing on a single file.

## Requirements

* ffmpeg.exe and ffprobe.exe must be added to PATH. You can download them here: https://ffmpeg.org/download.html
* .NET 6 must be installed.

## Usage

Using a built executable from the ICut project ([icut.exe](https://github.com/lewisc64/CutLang/releases/latest)):

`icut VIDEO PROGRAM [flags?]`

Example:

`icut video.mp4 "00:30-01:00"`

Output would be saved to `video_cut.mp4`.

### Flags

* --vertical: crops output video to a 9:16 ratio.

### Queries

Currently supports the following:

* Cutting
  * Extract video between 30 seconds and 1 minute: `00:30-01:00`
  * Extract video between a precise time and the end of the video: `32:12.325-END`
    * Note: this will only cut on a video keyframe, so may appear inaccurate.
* Concatenating
  * Join two cuts together: `00:30-01:00 + 02:00-03:00`
* Speeding up
  * 5x speed: `00:30-01:00 >> 5`
* Slowing down
  * 0.2x speed (5 times as slow): `00:30-01:00 << 5`
* Brackets
  * Concatenate two cuts, then speed them up by 2x: `(00:30-01:00 + 02:00-03:00) >> 2`
