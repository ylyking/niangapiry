
using UnityEngine;
using System.Collections;

public class Conversation : MonoBehaviour {

//[script RequireComponent(BoxCollider);
//@script RequireComponent(OnGUIShit);

public TextAsset ConversationFile;
public string NameId;
public GameObject PlayerRef = null;
public  AudioClip soundChat;
private CameraTargetAttributes PlayerCamera;
//private bool  CharacterActive = false;


void  Start (){
	if ( PlayerRef != null)
        PlayerCamera = PlayerRef.GetComponent<CameraTargetAttributes>();
	if (  ConversationFile != null )
		Managers.Dialog.Init( ConversationFile );
	else 
	{
		 Debug.Log ( "Conversation XML not assigned"); 
		 return;
	}

}


void  OnTriggerEnter (  Collider other   ){
    if (!Managers.Dialog.IsInConversation() && other.CompareTag("Player"))
	{
		if (PlayerCamera)
		{
			PlayerCamera.Offset.y = 1.0f;
            PlayerCamera.Offset.x = (PlayerRef.GetComponent<PlayerControls>() as PlayerControls).orientation;
			PlayerCamera.distanceModifier = 2.0f;
		}
		
        //CharacterActive = true;
		
		if ( NameId != null )	
		{	
			Managers.Audio.Play( soundChat, gameObject.transform, 1f, 2.0f);

            Managers.Dialog.StartConversation(NameId);
		}
		else Debug.Log ( "Conversation ID not assigned");
		
	}
}

void  OnTriggerExit (  Collider other   ){
    if (!Managers.Dialog.IsInConversation() && other.CompareTag("Player"))
	{
        //CharacterActive = false;
	
		if (PlayerCamera)
		{
            PlayerCamera.Offset.y = 0.0f;
			PlayerCamera.Offset.x = 0.0f;
			PlayerCamera.distanceModifier = 2.5f;
		}
	}
}

}
