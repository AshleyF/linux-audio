# Audio Capture/Playback

Using [ALSA (Advanced Linux Sound Architecture)](http://alsa-project.org).

    apt install libasound2-dev

To determine available hardware (e.g. "plughw:0,0" means card 0, device 0):

	arecord --list-devices	
