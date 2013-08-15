using UnityEngine;
using System.Collections;

public class MoviePlayer : MonoBehaviour {


    public MovieTexture video;

    void Start()
    {
        video = renderer.material.mainTexture as MovieTexture;
    }

    void Update()
    {
	if (Input.GetButtonDown ("Jump")) 
    {
        if (video.isPlaying)
        {
            video.Pause();
		}
		else
        {
            video.Play();
		}
	}
    }
	
}
