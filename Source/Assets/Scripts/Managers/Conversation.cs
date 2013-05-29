
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Conversation : MonoBehaviour {


public TextAsset ConversationFile;
public string NameId;
public string oneShotId;

public GameObject PlayerRef = null;
public  AudioClip soundChat;

public bool OneShot = true;
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
		 Debug.Log ( "Conversation XML not assigned"); 
		 return;
	}
    //enabled = false;

}

void Update()
{
    if (Managers.Dialog.IsInConversation())
    {
        OneShot = false;

        if (PlayerCamera)
        {

            PlayerCamera.Offset.y = 1;
            PlayerCamera.Offset.x = (transform.position.x - Managers.Game.PlayerPrefab.transform.position.x) * .5f;
            PlayerCamera.distanceModifier = 2;
        }

    }
    else if (soundSource && soundSource.isPlaying)
    {
        soundSource.Stop();
        Destroy(soundSource);
    }

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

            if (oneShotId != null)
            {
                soundSource = Managers.Audio.Play(soundChat, gameObject.transform, 1f, 2);
                Managers.Dialog.StartConversation(oneShotId);
            }
            else Debug.Log("Conversation ID not assigned");

        }
    }
}

void OnTriggerStay(Collider hit)
{
    if (hit.tag == "Player" && Managers.Game.InputUp && !Managers.Dialog.IsInConversation())
    {
        if (NameId != null)
        {
            soundSource = Managers.Audio.Play(soundChat, gameObject.transform, 1f, 2.0f);
            Managers.Dialog.StartConversation(NameId);
        }
        else Debug.Log("Conversation ID not assigned");
    }
}

void  OnTriggerExit (  Collider other   )
{
    if (other.CompareTag("Player") )
	{
        Managers.Dialog.StopConversation();
        //Managers.Dialog.DeInit();

	
		if (PlayerCamera)
		{
            PlayerCamera.Offset.y = 0;
			PlayerCamera.Offset.x = 0;
			PlayerCamera.distanceModifier = 2.5f;

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
