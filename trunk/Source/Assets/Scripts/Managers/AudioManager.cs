
using UnityEngine;
using System.Collections;
 
public class AudioManager : MonoBehaviour
{
	public bool SoundEnable = true;
	public bool _MusicEnable = true;
	public AudioSource Music;
	
	public bool MusicEnable
	{
		get
		{
			return this._MusicEnable;
		}
		set
		{
			if (value == this._MusicEnable)
				return;
			this._MusicEnable = value;
			if (this._MusicEnable)
				Managers.Audio.ResumeMusic();
			else
				Managers.Audio.PauseMusic();
		}
	}

	void Awake()
	{
        if (Camera.main != null)
        {
            //Music = gameObject.AddComponent<AudioSource>();
            Music =(Camera.main.gameObject).AddComponent<AudioSource>();
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

		GameObject go = new GameObject("Audio: " + clip.name);
		go.transform.position = emitter.position;
		go.transform.parent = emitter;
		
		AudioSource audioSrc = go.AddComponent<AudioSource>();
		audioSrc.clip = clip;
		audioSrc.volume = volume;
		audioSrc.pitch = pitch;
		audioSrc.Play();
		Object.Destroy( go, clip.length);
		return audioSrc;
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

	public AudioSource PlayMusic(AudioClip clip, float volume = 1, float pitch =1)
    {
		if (!MusicEnable)
			return null;
		
		if (Music.clip == clip)
			return Music;
		
		if (Music.isPlaying)
			Music.Stop();
		
		Music.clip = clip;
		Music.volume = volume;
		Music.pitch = pitch;
		Music.Play();
		
		return Music;
    }
	
	public void PauseMusic()
	{
		if (!Music.isPlaying)
			return;
		this.Music.Pause();
	}
	
	public void ResumeMusic()
	{
		if (Music.isPlaying)
			return;
		this.Music.Play();
	}
	
	public AudioSource StopMusic()
    {
		if ( Music.isPlaying )
			Music.Stop();
		
        return Music;
    }

    public void CheckAllSound( bool active)
    {
		if (active)
			return;

		foreach (AudioSource audioSource in Object.FindObjectsOfType(typeof (AudioSource)))
		{
			if (audioSource != Music)
				Object.Destroy(audioSource);
		}
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