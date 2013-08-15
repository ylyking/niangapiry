
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Conversation : MonoBehaviour {


public TextAsset ConversationFile;
public string NameId;
public string oneShotId;
public float zoom = 2;
public float oldZoom = 2;

public GameObject PlayerRef = null;
public  AudioClip soundChat;

public bool OneShot = true;
public bool isTalking = false;
public bool isPressing = false;
private AudioSource soundSource;
private CameraTargetAttributes PlayerCamera;

void  Start ()
{
    if (!PlayerRef && Managers.Game.PlayerPrefab)
        PlayerRef = Managers.Game.PlayerPrefab ;
	if ( PlayerRef != null )
        PlayerCamera = PlayerRef.GetComponent<CameraTargetAttributes>();

	if (  ConversationFile != null )
		Managers.Dialog.Init( ConversationFile );
	else 
	{
		 Debug.Log ( "Conversation file XML not assigned"); 
		 return;
	}
    //enabled = false;

}

void Update()
{
    if (isTalking && Managers.Dialog.IsInConversation())
    {
        OneShot = false;

        if (PlayerCamera)
        {

            PlayerCamera.Offset.y = 1;
            //PlayerCamera.Offset.x = (transform.position.x - Managers.Game.PlayerPrefab.transform.position.x) * .5f;
            PlayerCamera.Offset.x = (transform.position.x - Managers.Game.PlayerPrefab.transform.position.x) * .5f;
            PlayerCamera.distanceModifier = zoom;
        }

    }
    else if (soundSource && soundSource.isPlaying)
    {
        soundSource.Stop();
        Destroy(soundSource);
    }


    if ( !Managers.Dialog.IsInConversation())  // This it's the OneCall Conversation Activation Fix 
    {
        // This it's the OneCall Conversation De-Activation Fix 
        if (Input.GetButtonUp("Fire1"))
        {
            isTalking = false;
            isPressing = false;
        }

        if (Input.GetButtonDown("Fire1") && !isTalking)
        {
            if (isPressing == false)
            {
                // Call your event function here.
                isPressing = true;
            }
        }
    }

    //if (!isTalking && !Managers.Dialog.IsInConversation())  // This it's the OneCall Conversation Activation Fix 
    //{
    //    //if (Input.GetAxisRaw("Fire1") != 0)
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        if (isPressing == false)
    //        {
    //            // Call your event function here.
    //            isPressing = true;
    //        }
    //    }
    //}


}


void  OnTriggerEnter (  Collider other   )
{
    if (other.CompareTag("Player"))
    {

        if (!PlayerRef && Managers.Game.PlayerPrefab)
            PlayerRef = Managers.Game.PlayerPrefab;
        if (PlayerRef != null)
            PlayerCamera = PlayerRef.GetComponent<CameraTargetAttributes>();

        if (OneShot && !Managers.Dialog.IsInConversation())
        {
            OneShot = false;
            Input.ResetInputAxes();

            if (oneShotId != null)
            {
                //if (PlayerCamera)
                //if (zoom != PlayerCamera.distanceModifier)
                    oldZoom = PlayerCamera.distanceModifier;

                isTalking = true;
                soundSource = Managers.Audio.Play(soundChat, gameObject.transform, 1f, 2);
                Managers.Dialog.StartConversation(oneShotId);
            }
            else Debug.Log("Conversation ID not assigned");

        }
    }
}

void OnTriggerStay(Collider hit)
{

    if (hit.tag == "Player" )
    {
        //if ((Managers.Game.InputUp || isPressing) && !Managers.Dialog.IsInConversation() && !isTalking)
        if ( isPressing && !Managers.Dialog.IsInConversation() && !isTalking)
        {

            if (NameId != null)
            {
                //Input.ResetInputAxes();

                //if (PlayerCamera)
                if (zoom != PlayerCamera.distanceModifier)
                    oldZoom = PlayerCamera.distanceModifier; // A Very Important Check to avoid Camera Zoom Fixation issues

                isTalking = true;
                soundSource = Managers.Audio.Play(soundChat, gameObject.transform, 1f, 2.0f);
                Managers.Dialog.StartConversation(NameId);
            }
            else Debug.Log("Conversation ID not assigned");
        }

        //if ( !Managers.Dialog.IsInConversation() )
        //{
        //    // This it's the OneCall Conversation De-Activation Fix 

        //    if (Input.GetAxisRaw("Fire1") == 0)
        //    //if ( Input.GetButtonUp("Fire1") )
        //    {
        //        isTalking = false;
        //        isPressing = false;
        //    }
        //}

    }
}

void  OnTriggerExit (  Collider other   )
{
    if (other.CompareTag("Player") )
	{
        isTalking = false;
        isPressing = false;

        Managers.Dialog.StopConversation();
        //Managers.Dialog.DeInit();
	
		if (PlayerCamera)
		{
            PlayerCamera.Offset.y = 0;
			PlayerCamera.Offset.x = 0;
			PlayerCamera.distanceModifier = oldZoom;

            if (soundSource && soundSource.isPlaying)
            {
                soundSource.Stop();
                Destroy(soundSource);
            }
		}
        //enabled = false;
	}
}


void OnBecameVisible()
{
    enabled = true;
}

void OnBecameInvisible()
{
    enabled = false;
}

void OnDestroy()
{
    Managers.Dialog.DeInit();
}

//void OnDestroy()
//{
//        Managers.Dialog.StopConversation();

//        if (PlayerCamera)
//        {
//            PlayerCamera.Offset.y = 0.0f;
//            PlayerCamera.Offset.x = 0.0f;
//            PlayerCamera.distanceModifier = 2.5f;

//            if (soundSource && soundSource.isPlaying)
//            {
//                soundSource.Stop();
//                Destroy(soundSource);
//            }
//        }
//}


}
