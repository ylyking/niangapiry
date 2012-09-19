#pragma strict
#pragma downcast

import System.Collections.Generic;
import System.IO;

public  var SoundTrack		: AudioClip;

function Start()
{
	audio.clip = SoundTrack;
}

public function SetSoundtrack( soundTrack : AudioClip )
{
	audio.clip = soundTrack;
}

function Update()
{
	if (!audio.isPlaying && audio.clip.isReadyToPlay)
	{
	    audio.Play();
	}
	else
	{
	    Debug.Log("waiting - isplaying : " + audio.isPlaying + " isreadyToPlay : " + audio.clip.isReadyToPlay);
	}
}

//function StartMusic(  soundTrack : AudioClip )
//{
//	audio.clip = soundTrack;
//	while(true)
//		yield PlayMusic();
//}
//
//function PlayMusic()
//{
//	if (!audio.isPlaying && audio.clip.isReadyToPlay)
//	{
//	    audio.Play();
//	}
//	else
//	{
//	    Debug.Log("waiting - isplaying : " + audio.isPlaying + " isreadyToPlay : " + audio.clip.isReadyToPlay);
//	}
//}