
using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;


public class ConversationManager : MonoBehaviour {

	GUISkin gSkin;
	Texture2D ComicTex ;	
	Rect ComicPos ;	
	Rect ComicCoord ;
    int miChatDirection = 0;														//  indicates the current Dialog's direction of Comic balloon 

 	List<cCharacterSpeaker> mSpeakers = new List<cCharacterSpeaker>();// Characters talk able list
    List<cConversation> mConversations = new List<cConversation>();		// Conversations list
	cConversationNode mpCurrentConversationNode;
		
  	bool  mbConversationState = false;
    float mfMessageTime;
    uint muiChooseOption = 0;														// Actual choice
    CameraTargetAttributes CamTarget = null;

    //	cConversation cConversationIt;
    //	cConversation cCIterator	;

    public void  Init (  TextAsset lacConversationFile   ){
    	gSkin = Resources.Load ("GUI/GUISkin") as GUISkin;
		ComicTex = Resources.Load("GUI/Rainbow") as Texture2D;
 		ComicPos = new Rect( (Screen.width * .24f), (Screen.height * .05f), ComicTex.width *1.02f , ComicTex.height *.5f);
 		ComicCoord = new Rect( 0, 0, .5f, .25f);

        if ( CamTarget == null && Managers.Game.PlayerPrefab )
        CamTarget = Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>() as CameraTargetAttributes;
 
    
    	XmlDocument Doc = new XmlDocument();
   		Doc.LoadXml(lacConversationFile.text);
   		

        if ( Doc != null && (Doc.DocumentElement.Name == "Conversations") )
        {
        	// Read all the Characters
        	foreach ( XmlNode CharacterInfo in Doc.GetElementsByTagName("Character") )	// array of the level nodes.
    		{
				this.ParseCharacter( CharacterInfo );
   			}
   			
 			// Read all the conversations 
        	foreach ( XmlNode ConversationInfo in Doc.GetElementsByTagName("Conversation") )	// array of the level nodes.
    		{
            	ParseConversation( ConversationInfo );  			
   			}
   			
   			mpCurrentConversationNode = null;
  			muiChooseOption = 0;
        }   	
   	}       
        //////////////////////////////////////////////////////////////
        
    public void  DeInit (){;}
        //////////////////////////////////////////////////////////////
        
  	public void  ParseCharacter (  XmlNode lpElem   ){
  		cCharacterSpeaker lCharacter = new cCharacterSpeaker();
  		
    			
      	//Character ID
       	lCharacter.miCharacterId =  int.Parse( lpElem.Attributes["speakerId"].Value) ;  
//        if(lCharacter.miCharacterId >= 0); 	

       	// Character Name
      	string lacName = lpElem.InnerText;
//      	Debug.Log( lacName );

//      assert(lacName);
       	lCharacter.macCharacterName = lacName;

       	// Add the character to the Vector
      	mSpeakers.Add( lCharacter );
    }
        //////////////////////////////////////////////////////////////      
          
   	public void  ParseConversation (  XmlNode lpElem   ){
  		cConversation lConversation = new cConversation();
  		lConversation.mRoot = new cConversationNode();	
		
        // Read the conversation ID
        lConversation.macName = lpElem.Attributes["nameId"].Value ;// guardando el nombre de la conversación
//        assert(lacNameId);

        // Read the tree
        ParseNode( lpElem.FirstChild, lConversation.mRoot  );// guardando la ruta de la conversación

        // Add the conversations to the list
        mConversations.Add( lConversation );// guardando en la lista la conversación;
        
  	}
        //////////////////////////////////////////////////////////////
           
 	public void  ParseNode (  XmlNode lpElem ,   cConversationNode lpCurrentNode   ){	  
	  //Base case
	  if (lpElem == null)
	  {
		   lpCurrentNode.meType = eConversationNodeType.eEndConversation;
		   return;
	  }
	  
	  //Get the type of node from the XML
	  string lacTag = lpElem.Name;
	  
	  if (lacTag == "Talk")
	  {
        // Set the type
        lpCurrentNode.meType = eConversationNodeType.eNormalTalk;

        // Read the text
        string lacText = lpElem.InnerText;
        
//        assert(lacText);
        lpCurrentNode.macText = lacText;

        // Read the speaker Id
        lpCurrentNode.miCharacterId = int.Parse( lpElem.Attributes["speakerId"].Value);
        
        
        if (lpCurrentNode.miCharacterId < 0) Debug.LogError("error CharacterId < 0");
        if (lpCurrentNode.miCharacterId > mSpeakers.Count) Debug.LogError("error CharacterId > size");
        
        // Read the time
         lpCurrentNode.mfDuration = float.Parse( lpElem.Attributes["time"].Value );
         
        //Prepare the next node
        cConversationNode lNodeTalk = new cConversationNode();
        lpCurrentNode.mChildren.Add( lNodeTalk ); // Asignando un puntero del Nodo a la lista 
        
        // Continue the recursivity
        ParseNode( lpElem.NextSibling, lpCurrentNode.mChildren[0] );
	  }
	  else if (lacTag == "ChooseTalk")
	  {
        // Set the type
        lpCurrentNode.meType = eConversationNodeType.eChooseTalk;

        // Read all the options
        for ( XmlNode pElem2 = lpElem.FirstChild; pElem2 != null; pElem2 = pElem2.NextSibling)
        {
	        if (pElem2 == null) 			Debug.Log("error pElem2 null ");
	        if (pElem2.Name != "Option" ) 	Debug.Log("error pElem2.Name !=  Option");
	
	        //Add a node to the vector
	        cConversationNode lNodeChoose = new cConversationNode();
	        lpCurrentNode.mChildren.Add( lNodeChoose ); 
	
	        // Continue the recursivity
	        uint luiLastIndex = (uint)lpCurrentNode.mChildren.Count -1;  // 
	        ParseNode( pElem2.FirstChild, (lpCurrentNode.mChildren[ (int)luiLastIndex ]) );
        }
	  }
	  else
	  {
	    //Invalid tag found
	    Debug.Log("Wrong tag\n");
	    return;
	  }
  	}
        //////////////////////////////////////////////////////////////    
       
    public void  NextMessage (  uint luiNextMessageIndex   ){
	        // Checks to make sure that all is right
        if ( mpCurrentConversationNode == null)	Debug.Log("error mpCurrentConversationNode null ");
        if (luiNextMessageIndex > mpCurrentConversationNode.mChildren.Count ) 	Debug.Log("luiNextMessageIndex >  ...mChildren.Count");
	
	    // Select the next node
	    bool  lbIsChooseNode = (mpCurrentConversationNode.meType == eConversationNodeType.eChooseTalk );
	    mpCurrentConversationNode =  (mpCurrentConversationNode.mChildren[ (int)luiNextMessageIndex ]);
	
	    // If the conversation have finished 
	    if ( mpCurrentConversationNode.meType == eConversationNodeType.eEndConversation )
	    {
            mpCurrentConversationNode = null;
	    }
	    else if ( lbIsChooseNode )
	    {
            // This node is the option text
            // We need to choose the next child     NextMessage(0);
            NextMessage(0);
	    }
	    else
	    {
            //Set the right input
            muiChooseOption = 0;
            mfMessageTime = mpCurrentConversationNode.mfDuration;
    /*        mbConversationState = false;
            Debug.Log( "NextMessage: setting mbConversation in false" );*/
	    }
    }
        //////////////////////////////////////////////////////////////       

    public IEnumerator CoUpdate(  float lfTimestep   ) //: IEnumerator
    {
//    	if(Input.GetButton("Fire1") )Debug.Log( "\n PRESSING ENTER KEY ");  
        while (this.IsInConversation())
        {

            if (!mbConversationState) yield return 0;

            if (CamTarget.Offset.x > 0)                                                     // Check Player Position and Id
                miChatDirection = System.Convert.ToByte(miChatDirection != 0);              // the Player is on left side
            else
                miChatDirection = System.Convert.ToByte(miChatDirection == 0);              // else the Player is on right 


            ComicCoord = new Rect(.5f * miChatDirection, 0, (miChatDirection > 0 ? -.5f : .5f), .25f);


            if (mpCurrentConversationNode != null)
            {
                switch (mpCurrentConversationNode.meType)
                {
                    case eConversationNodeType.eNormalTalk:									// Enum 0; Nodo activo de tipo eNormalTalk

                        mfMessageTime -= lfTimestep * .5f;	                					// Decrease the message time

                        if ((mfMessageTime <= 0.0f) || Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
                            NextMessage(0);               								//Need to continue with the next message or node
                        break;

                    case eConversationNodeType.eChooseTalk:									// Enum 1; Nodo activo de tipo eChooseTalk

                        if (Input.GetKeyDown("up") && (muiChooseOption > 0))
                            muiChooseOption--;
                        if (Input.GetKeyDown("down") && (muiChooseOption < (mpCurrentConversationNode.mChildren.Count - 1)))
                            muiChooseOption++;
                        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetKeyDown("return"))
                            NextMessage(muiChooseOption);
                        break;

                    default:																// Enum 2; Nodo activo de tipo eEndConversation
                        mpCurrentConversationNode = null;
                        break;
                }
            }
            else mbConversationState = false; // If we have a null mpCurrentConversationNode then it´s the end

            yield return 0;
        }
	    
    }
        //////////////////////////////////////////////////////////////     
        
    public void  StartConversation ( string lacConversationId   ){
        mbConversationState = true; 									// stops Player moves during Conversation
    
		foreach( cConversation lpIterator in mConversations )
	    {       
            if (lpIterator.macName == lacConversationId )				// If we have a talking with the correct Character Id...
            {   
                mpCurrentConversationNode = lpIterator.mRoot; 			// Get access Root
                
                mfMessageTime = mpCurrentConversationNode.mfDuration; 	// Asignamos el tipo inicial
                
                muiChooseOption = 0;
            }
	    }

        StartCoroutine(CoUpdate(Time.deltaTime));
	    
    } // con esto llamamos al Conversation Manager desde Statement.cpp
        //////////////////////////////////////////////////////////////
        
 	public void  Render (){
	
	 if (mbConversationState && mpCurrentConversationNode != null )// Si Hay Conversación y el nodo no es NULL
     {
	
		if ( gSkin )		
		GUI.skin = gSkin;

        GUI.color = Color.white;
        GUI.skin.label.fontSize = 16;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.DrawTextureWithTexCoords( ComicPos, ComicTex,  ComicCoord ); 								// Dibujar Globito de COMIC 
		GUI.color = new Color(.322f, .306f, .631f, 1);
              
        switch ( mpCurrentConversationNode.meType)
        {
            case eConversationNodeType.eNormalTalk: 													// Enum 0; Nodo activo de tipo eNormalTalk

				GUI.Label ( new Rect( (Screen.width * .5f) -220, (Screen.height * .1f) , 486, 200), 
					 mSpeakers[mpCurrentConversationNode.miCharacterId].macCharacterName + ":"  );		// Show character Id Dialog
					 
				miChatDirection = mpCurrentConversationNode.miCharacterId % 2;
                                
				GUI.Label ( new Rect( (Screen.width * .5f) -220 , (Screen.height * .15f) , 486, 200), mpCurrentConversationNode.macText );// Character Speak														///*///
            	break;
	                
           	case eConversationNodeType.eChooseTalk: 													// Enum 1; Nodo activo de tipo eChooseTalk
	
                cConversationNode lcpChoice = (mpCurrentConversationNode.mChildren[(int) muiChooseOption ]) as cConversationNode;// Choosen option

				GUI.Label ( new Rect( (Screen.width * .5f) -220, (Screen.height * .1f) , 486, 200), 
					 mSpeakers[lcpChoice.miCharacterId].macCharacterName + ":"  );						// Show character Id Questions..
					 
				miChatDirection = lcpChoice.miCharacterId % 2;
                
                for (int liOptionIndex = 0; liOptionIndex < (mpCurrentConversationNode.mChildren.Count );  liOptionIndex++ )
                {
                    cConversationNode lcpOptions  = ( mpCurrentConversationNode.mChildren[ liOptionIndex ]);//Show posible options

                    if( lcpOptions != lcpChoice)// Printing the unselected Options
                    {

						GUI.color = new Color(.322f, .306f, .631f, 1);
                    	GUI.Label ( new Rect( (Screen.width * .5f)-220 , (Screen.height * .15f)+ liOptionIndex * 20, 486, 200), lcpOptions.macText );
                    }
                    else	if( lcpOptions == lcpChoice) // Printing the Choice
                    {
                        if (((int)(Time.time * 5)) % 2 == 0)
                            GUI.color = new Color(1, 0.36f, 0.75f, 1);
     
                        else 
    						GUI.color = Color.magenta;


//						GUI.color = Color(1, 0.36f, 0.22f, 1);
						
                    	GUI.Label ( new Rect( (Screen.width * .5f)-220 , (Screen.height * .15f) + liOptionIndex * 20, 486, 200),  lcpOptions.macText );
                    }
                }
            	break;
        }
    }
     GUI.color = Color.white;

 }
        //////////////////////////////////////////////////////////////
               
 	public bool IsInConversation ()
    {
	   return (mbConversationState) ; 
    }

    public void StopConversation()
    {
        mpCurrentConversationNode = null;
        mbConversationState = false;
    }
        //////////////////////////////////////////////////////////////
      
}


public enum eConversationNodeType 																	// conversatión type
{
    eNormalTalk,
    eChooseTalk,
    eEndConversation
};

public struct cCharacterSpeaker 																// game Character talks
{
    public int miCharacterId;
    public string macCharacterName;
};

public class cConversationNode 																// Conversation SubNodes
{
    public eConversationNodeType meType;
    public string macText;
    public float mfDuration;
    public int miCharacterId;

    public List<cConversationNode> mChildren = new List<cConversationNode>();
    //  std::vector<cConversationNode> mChildren;
};

public class cConversation 																	// Conversations tree roots , only one in here
{
    public string macName;
    public cConversationNode mRoot;
};