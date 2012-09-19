#pragma strict

@script RequireComponent(BoxCollider);
//@script RequireComponent(OnGUIShit);

public var ConversationFile : TextAsset;
public var NameId			: String;
public var PlayerRef 		: GameObject;
public  var soundChat		: AudioClip;
private var PlayerCamera	: CameraTargetAttributes;
private var CharacterActive	: boolean = false;



//private var ActiveConversation 	: boolean = false;


function Start ()
{
	if ( PlayerRef != null)
		PlayerCamera = PlayerRef.GetComponent(CameraTargetAttributes);
	if (  ConversationFile != null )
		ConversationManager.Get().Init( ConversationFile );
	else 
	{
		 Debug.Log ( "Conversation XML not assigned"); 
		 return;
	}
	
	while(true)
		yield CoUpdate();

}

function CoUpdate () : IEnumerator
{
//	if (  Input.GetButtonUp( "Jump") && !ConversationManager.Get().IsInConversation() )
//	{
//		if ( NameId != null )		
//			ConversationManager.Get().StartConversation( NameId );		
//	}
	
	if (ConversationManager.Get().IsInConversation() && this.CharacterActive )
		ConversationManager.Get().Update(Time.deltaTime);
	
}


function OnTriggerEnter( other : Collider )
{
	if ( !ConversationManager.Get().IsInConversation() && other.CompareTag("Player") )
	{
		if (PlayerCamera)
		{
			PlayerCamera.heightOffset = 1.0;
			PlayerCamera.widthOffset = (PlayerRef.GetComponent(PlayerControls)as PlayerControls).orientation;
			PlayerCamera.distanceModifier = 2.0;
		}
		
		CharacterActive = true;
		
		if ( NameId != null )	
		{	
			AudioManager.Get().Play(soundChat, gameObject.transform, 1f, 2.0);
		
			ConversationManager.Get().StartConversation( NameId );
		}
		else Debug.Log ( "Conversation ID not assigned");
		
	}
}

function OnTriggerExit( other : Collider )
{
	if ( !ConversationManager.Get().IsInConversation() && other.CompareTag("Player") )
	{
		CharacterActive = false;
	
		if (PlayerCamera)
		{
			PlayerCamera.heightOffset = 0.0;
			PlayerCamera.widthOffset = 0.0;
			PlayerCamera.distanceModifier = 2.5;
		}
	}
}
