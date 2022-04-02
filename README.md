# CutLang
 Language describing how to cut up videos

## Requirements

* FFMPEG.exe must be added to PATH.

## Usage

Using a built executable from the ICut project:

`icut VIDEO PROGRAM`

Example:

`icut video.mp4 "00:30-01:00"`

Output would be saved to `video_cut.mp4`.

### Queries

Currently supports the following:

* Cutting - `00:30-01:00`, `32:12.325-END`
* Concatenating - `00:30-01:00 + 02:00-03:00`
