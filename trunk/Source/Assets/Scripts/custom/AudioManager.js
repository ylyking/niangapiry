#pragma strict
#pragma downcast

import System.Collections.Generic;
import System.IO;

public class AudioManager
{
	private static var Instance : AudioManager = null;
    private function AudioManager() {;} 												// private Constructor protected
    public static function Get(): AudioManager 										// Singleton Instance Accessor 
    {
    	if(Instance == null)
    		Instance = new AudioManager();
        return Instance;
    }


    public function Play( clip : AudioClip, emitter : Transform) : AudioSource
    {
        return Play(clip, emitter, 1f, 1f);
    }
 
    public function Play( clip : AudioClip, emitter : Transform, volume : float) : AudioSource
    {
        return Play(clip, emitter, volume, 1f);
    }

    public function Play( clip : AudioClip, emitter : Transform, volume : float, pitch : float) : AudioSource
    {
        //Create an empty game object
        var go : GameObject = new GameObject( "Audio: " +  clip.name);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;
 
        //Create the source
        var source : AudioSource = go.AddComponent.<AudioSource>() as AudioSource;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        GameObject.Destroy( go, clip.length);
        return source;
    }
 
   
    public function PlayLoop( clip : AudioClip, emitter : Transform, volume : float, pitch : float) : AudioSource
    {
        //Create an empty game object
        var go : GameObject = new GameObject( "Audio: " +  clip.name);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;
 
        //Create the source
        var source : AudioSource = go.AddComponent.<AudioSource>() as AudioSource;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = true;
        source.Play();
//        GameObject.Destroy( go, clip.length);
        return source;
    }  
    /// <summary>
    /// Plays a sound by creating an empty game object with an AudioSource
    /// and attaching it to the given transform (so it moves with the transform). Destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="emitter"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>   
    
 
    public function Play( clip : AudioClip, point : Vector3) : AudioSource
    {
        return Play(clip, point, 1f, 1f);
    }
 
    public function Play( clip : AudioClip,  point : Vector3, volume : float) : AudioSource
    {
        return Play(clip, point, volume, 1f);
    }
 
    public function Play( clip : AudioClip,  point : Vector3,  volume : float, pitch : float) : AudioSource
    {
        //Create an empty game object
        var go : GameObject = new GameObject("Audio: " + clip.name);
        go.transform.position = point;
 
        //Create the source
        var source : AudioSource = go.AddComponent(AudioSource);
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        GameObject.Destroy( go, clip.length);
        return source;
    }
    
    /// <summary>
    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
    /// in that place and destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="point"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns> 
    
          
    
//    function PlaySound ( soundName : AudioClip, soundDelay : float)
//	{
//		if ( !audio.isPlaying && Time.time > soundRate )
//		{
//			soundRate = Time.time + soundDelay;
//			audio.clip = soundName;
//			audio.Play();
//			yield WaitForSeconds ( audio.clip.length );
//		}
//	}



function StartMusic( soundTrack : AudioClip, emitter : Transform )
{
	var music = Play( soundTrack, emitter);
	

	while(!MusicStoped)
		yield PlayMusic( music );
}
private var MusicStoped : boolean = false;

function StopMusic()
{
	MusicStoped = true;
}

function PlayMusic( soundTrack : AudioSource )
{

	if (!soundTrack.isPlaying && soundTrack.clip.isReadyToPlay)
	{
	    soundTrack.Play();
	}
	else
	{
	    Debug.Log("waiting - isplaying : " + soundTrack.isPlaying + " isreadyToPlay : " + soundTrack.clip.isReadyToPlay);
	}
	yield;
}

};