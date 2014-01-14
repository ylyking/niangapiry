
using UnityEngine;
using System.Collections;
 
public class AudioManager : MonoBehaviour
{
    public bool SoundEnable = true;
    
    public bool _MusicEnable = true;
    public bool MusicEnable
    {
        get
        {
            return _MusicEnable;
        }

        set
        {
            if (value != _MusicEnable)
            {
                _MusicEnable = value;

                if (_MusicEnable)
                    Managers.Audio.ResumeMusic();
                else
                    Managers.Audio.PauseMusic();
            }
        }
    }

	public AudioSource Music = null;
	
	void Awake()
	{
        if (Camera.mainCamera != null)
        {
            //Music = gameObject.AddComponent<AudioSource>();
            Music = (AudioSource)(Camera.mainCamera.gameObject).AddComponent<AudioSource>();
            Music.playOnAwake = false;
            Music.Stop();
            Music.loop = true;
        }
	}
	
    public AudioSource Play(AudioClip clip, Transform emitter)
    {
        return Play(clip, emitter, 1f, 1f);
    }
 
    public AudioSource Play(AudioClip clip, Transform emitter, float volume)
    {
        return Play(clip, emitter, volume, 1f);
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
    public AudioSource Play(AudioClip clip, Transform emitter, float volume, float pitch)
    {
        if (!SoundEnable)
            return null;

        //Create an empty game object
        GameObject go = new GameObject ("Audio: " +  clip.name);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;
 
        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play ();
        Destroy (go, clip.length);
        return source;
    }
 
    public AudioSource Play(AudioClip clip, Vector3 point)
    {
        return Play(clip, point, 1f, 1f);
    }
 
    public AudioSource Play(AudioClip clip, Vector3 point, float volume)
    {
        return Play(clip, point, volume, 1f);
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
    public AudioSource Play(AudioClip clip, Vector3 point, float volume, float pitch)
    {
        if (!SoundEnable)
            return null;

        //Create an empty game object
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = point;
 
        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
	
	public AudioSource PlayMusic(AudioClip clip, float volume, float pitch)
    {
        if (!MusicEnable)
            return null;
		
		if ( Music.clip == null || (Music.clip).name.ToString() != clip.name.ToString() )
	    {
			if ( Music.isPlaying )
				Music.Stop();
			
	        //Create the source
			Music.clip = clip ;
	    	Music.Play();
		}
			        
		Music.volume = volume;
		Music.pitch = pitch;
		
        return Music;
    }
	
	public void PauseMusic()
    {
		if ( Music.isPlaying )
        	Music.Pause();
    }
	
	public void ResumeMusic()
    {
		if ( !Music.isPlaying )
        Music.Play();
    }
	
	public AudioSource StopMusic()
    {
		if ( Music.isPlaying )
			Music.Stop();
		
        return Music;
    }

    public void CheckAllSound( bool active)
    {
        if (!active)
        foreach (AudioSource sonidos in GameObject.FindSceneObjectsOfType(typeof(AudioSource)))
            if (sonidos != Music)
                Destroy(sonidos);
    }

    //public void CheckAllSound(bool FxActive, bool MusicActive)
    //{
    //    if (!MusicActive)
    //        PauseMusic();
    //    else
    //        ResumeMusic();

    //    if (!FxActive)
    //        foreach (AudioSource sonidos in GameObject.FindObjectsOfTypeAll(typeof(AudioSource)))
    //            if (sonidos != Music)
    //                Destroy(sonidos);
    //}
}