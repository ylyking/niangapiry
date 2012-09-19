#pragma strict


//public var aTexture 	: Texture2D;
public var gSkin 			: GUISkin;
public var PlayerTransform	: Transform;  


function Start()
{
//	gSkin.label.fontSize 	= 18;
}


function OnGUI()
{
	if(Event.current.type.Equals(EventType.Repaint))
	{
		if(gSkin)
			GUI.skin = gSkin;
			GUI.skin.label.fontSize = 16;
		    GUI.skin.label.fontStyle = FontStyle.Bold;
	
		ConversationManager.Get().Render();
	
	}

}