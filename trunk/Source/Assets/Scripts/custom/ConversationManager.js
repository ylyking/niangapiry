#pragma strict
#pragma downcast

import System.Collections.Generic;
import System.Xml;
import System.IO;
//import System;
import System.Text;


enum eConversationNodeType 																	// conversatión type
{
	eNormalTalk,
    eChooseTalk,
    eEndConversation
};

public class cCharacterSpeaker 																// game Character talks
{
    var miCharacterId 		: int						;
    var macCharacterName	: String					;
};

public class cConversationNode 																// Conversation SubNodes
{
    var meType 				: eConversationNodeType	;
    var macText				: String;
    var mfDuration			: float;
    var miCharacterId		: int;

 	var mChildren 			: List.<cConversationNode> 	= new List.<cConversationNode>();
//  std::vector<cConversationNode> mChildren;
};

public class cConversation 																	// Conversations tree roots , only one in here
{
    var macName				: String ;
    var mRoot				: cConversationNode;  
};

class ConversationManager 	//	: MonoBehaviour		
{
	private static var Instance : ConversationManager = null;
    private function ConversationManager() {;} 												// private Constructor protected
    public static function Get(): ConversationManager 										// Singleton Instance Accessor 
    {
    	if(Instance == null)
    		Instance = new ConversationManager();
        return Instance;
    }

	var gSkin				: GUISkin;
	var ComicTex			: Texture2D ;	
	var ComicPos			: Rect ;	
	var ComicCoord			: Rect ;
    var miChatDirection		: int = 0;														//  indicates the current Dialog's direction of Comic balloon 
	

 	var mSpeakers 			: List.<cCharacterSpeaker> = new List.<cCharacterSpeaker>();// Characters talk able list
 	var mConversations 		: List.<cConversation> = new List.<cConversation>();		// Conversations list

//	var cConversationIt 	: cConversation;
//	var cCIterator 			: cConversation	;
	
	var mpCurrentConversationNode : cConversationNode;
		
  	var mbConversationState : boolean = false;
    var mfMessageTime 		: float;
    var muiChooseOption		: uint = 0;														// Actual choice

    public function Init( lacConversationFile : TextAsset ) 
    {
    	gSkin = Resources.Load ("ñGUI/ÑGUISkin") as GUISkin;
		ComicTex = Resources.Load("ñGUI/Rainbow") as Texture2D;
 		ComicPos = Rect( (Screen.width * .26f), (Screen.height * .05f), ComicTex.width  , ComicTex.height *.5);
 		ComicCoord = Rect( 0, 0, .5, .25);
 
    
    	var Doc : XmlDocument = new XmlDocument();
   		Doc.LoadXml(lacConversationFile.text);
   		

        if ( Doc && (Doc.DocumentElement.Name == "Conversations") )
        {
        	// Read all the Characters
        	for ( var CharacterInfo	: XmlNode in Doc.GetElementsByTagName("Character") )	// array of the level nodes.
    		{
				this.ParseCharacter( CharacterInfo );
   			}
   			
 			// Read all the conversations 
        	for ( var ConversationInfo	: XmlNode in Doc.GetElementsByTagName("Conversation") )	// array of the level nodes.
    		{
            	ParseConversation( ConversationInfo );  			
   			}
   			
   			mpCurrentConversationNode = null;
  			muiChooseOption = 0;
        }   	
   	}       
        //////////////////////////////////////////////////////////////
        
    public function DeInit() {;}
        //////////////////////////////////////////////////////////////
        
  	public function ParseCharacter( lpElem : XmlNode ) 
  	{
  		var lCharacter : cCharacterSpeaker = new cCharacterSpeaker();
  		
    			
      	//Character ID
       	lCharacter.miCharacterId =  int.Parse( lpElem.Attributes["speakerId"].Value) ;  
//        if(lCharacter.miCharacterId >= 0); 	

       	// Character Name
      	var lacName : String = lpElem.InnerText;
//      	Debug.Log( lacName );

//      assert(lacName);
       	lCharacter.macCharacterName = lacName;

       	// Add the character to the Vector
      	mSpeakers.Add( lCharacter );
    }
        //////////////////////////////////////////////////////////////      
          
   	public function ParseConversation( lpElem : XmlNode ) 
  	{
  		var lConversation : cConversation = new cConversation();
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
           
 	public function ParseNode( lpElem : XmlNode, lpCurrentNode : cConversationNode )
  	{	  
	  //Base case
	  if (lpElem == null)
	  {
		   lpCurrentNode.meType = eConversationNodeType.eEndConversation;
		   return;
	  }
	  
	  //Get the type of node from the XML
	  var lacTag : String = lpElem.Name;
	  
	  if (lacTag == "Talk")
	  {
        // Set the type
        lpCurrentNode.meType = eConversationNodeType.eNormalTalk;

        // Read the text
        var lacText : String = lpElem.InnerText;
        
//        assert(lacText);
        lpCurrentNode.macText = lacText;

        // Read the speaker Id
        lpCurrentNode.miCharacterId = int.Parse( lpElem.Attributes["speakerId"].Value);
        
        
        if (lpCurrentNode.miCharacterId < 0) Debug.LogError("error CharacterId < 0");
        if (lpCurrentNode.miCharacterId > mSpeakers.Count) Debug.LogError("error CharacterId > size");
        
        // Read the time
         lpCurrentNode.mfDuration = float.Parse( lpElem.Attributes["time"].Value );
         
        //Prepare the next node
        var lNodeTalk : cConversationNode = new cConversationNode();
        lpCurrentNode.mChildren.Add( lNodeTalk ); // Asignando un puntero del Nodo a la lista 
        
        // Continue the recursivity
        ParseNode( lpElem.NextSibling, lpCurrentNode.mChildren[0] );
	  }
	  else if (lacTag == "ChooseTalk")
	  {
        // Set the type
        lpCurrentNode.meType = eConversationNodeType.eChooseTalk;

        // Read all the options
        for ( var pElem2 : XmlNode = lpElem.FirstChild; pElem2; pElem2 = pElem2.NextSibling)
        {
	        if (pElem2 == null) 			Debug.Log("error pElem2 null ");
	        if (pElem2.Name != "Option" ) 	Debug.Log("error pElem2.Name !=  Option");
	
	        //Add a node to the vector
	        var lNodeChoose : cConversationNode = new cConversationNode();
	        lpCurrentNode.mChildren.Add( lNodeChoose ); 
	
	        // Continue the recursivity
	        var luiLastIndex : uint = lpCurrentNode.mChildren.Count -1;  // 
	        ParseNode( pElem2.FirstChild, lpCurrentNode.mChildren[ luiLastIndex ] );
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
       
    public function NextMessage( luiNextMessageIndex : uint )
    {
	        // Checks to make sure that all is right
        if ( mpCurrentConversationNode == null)	Debug.Log("error mpCurrentConversationNode null ");
        if (luiNextMessageIndex > mpCurrentConversationNode.mChildren.Count ) 	Debug.Log("luiNextMessageIndex >  ...mChildren.Count");
	
	    // Select the next node
	    var lbIsChooseNode : boolean = (mpCurrentConversationNode.meType == eConversationNodeType.eChooseTalk );
	    mpCurrentConversationNode =  (mpCurrentConversationNode.mChildren[ luiNextMessageIndex ]);
	
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
            //Set the right values
            muiChooseOption = 0;
            mfMessageTime = mpCurrentConversationNode.mfDuration;
    /*        mbConversationState = false;
            Debug.Log( "NextMessage: setting mbConversation in false" );*/
	    }
    }
        //////////////////////////////////////////////////////////////       

    public function Update( lfTimestep : float ) //: IEnumerator
    {
//    	if(Input.GetButton("Fire1") )Debug.Log( "\n PRESSING ENTER KEY ");  
    
    	if ( !mbConversationState ) return;
    	
    	ComicCoord = Rect( .5 * miChatDirection, 0, (miChatDirection ? -.5 : .5), .25);
    	
	    if (mpCurrentConversationNode != null )
	    {
    		switch (mpCurrentConversationNode.meType)
            {
	            case eConversationNodeType.eNormalTalk:									// Enum 0; Nodo activo de tipo eNormalTalk
	            
	                mfMessageTime -= lfTimestep;	                					// Decrease the message time

	                if ( (mfMessageTime <= 0.0f) || Input.GetButtonDown( "Fire1" ) || Input.GetKeyDown("return")   )
	                        NextMessage(0);               								//Need to continue with the next message or node
                	break;
	            
	            case eConversationNodeType.eChooseTalk:									// Enum 1; Nodo activo de tipo eChooseTalk
	            
                    if( Input.GetKeyDown( "up" ) && ( muiChooseOption > 0 )  )
                             muiChooseOption--;                    
                    if( Input.GetKeyDown( "down" ) && ( muiChooseOption < ( mpCurrentConversationNode.mChildren.Count -1 ) ) )
                            muiChooseOption++;                                            
                    if (Input.GetButtonDown( "Fire1" ) || Input.GetButtonDown( "Jump" ) || Input.GetKeyDown("return")  )
                            NextMessage( muiChooseOption );
                	break;
	
	            default:																// Enum 2; Nodo activo de tipo eEndConversation
                    mpCurrentConversationNode = null ;
                	break;
            }
	    }
	    else mbConversationState = false; // If we have a null mpCurrentConversationNode then it´s the end
	    
    }
        //////////////////////////////////////////////////////////////     
        
    public function StartConversation(lacConversationId : String )
    {
        mbConversationState = true; 									// stops Player moves during Conversation
    
		for ( var lpIterator : cConversation in mConversations )
	    {       
            if (lpIterator.macName == lacConversationId )				// If we have a talking with the correct Character Id...
            {   
                mpCurrentConversationNode = lpIterator.mRoot; 			// Get access Root
                
                mfMessageTime = mpCurrentConversationNode.mfDuration; 	// Asignamos el tipo inicial
                
                muiChooseOption = 0;
            }
	    }
	    
    } // con esto llamamos al Conversation Manager desde Statement.cpp
        //////////////////////////////////////////////////////////////
        
 	public function Render() 
 {
	
	 if (mbConversationState && mpCurrentConversationNode != null )// Si Hay Conversación y el nodo no es NULL
     {
	
		if ( gSkin )		
		GUI.skin = gSkin;
   		GUI.skin.label.fontSize = 16;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.DrawTextureWithTexCoords( ComicPos, ComicTex,  ComicCoord ); 								// Dibujar Globito de COMIC 
		GUI.color = Color(.322, .306, .631, 1);
              
        switch ( mpCurrentConversationNode.meType)
        {
            case eConversationNodeType.eNormalTalk: 													// Enum 0; Nodo activo de tipo eNormalTalk

				GUI.Label (  Rect( (Screen.width * .5f) -200, (Screen.height * .1f) , 486, 200), 
					 mSpeakers[mpCurrentConversationNode.miCharacterId].macCharacterName + ":"  );		// Show character Id Dialog
					 
				miChatDirection = mpCurrentConversationNode.miCharacterId % 2;
                                
				GUI.Label (  Rect( (Screen.width * .5f) -200 , (Screen.height * .15f) , 486, 200), mpCurrentConversationNode.macText );// Character Speak														///*///
            	break;
	                
           	case eConversationNodeType.eChooseTalk: 													// Enum 1; Nodo activo de tipo eChooseTalk
	
                var lcpChoice : cConversationNode = (mpCurrentConversationNode.mChildren[ muiChooseOption ]) as cConversationNode;// Choosen option

				GUI.Label (  Rect( (Screen.width * .5f) -200, (Screen.height * .1f) , 486, 200), 
					 mSpeakers[lcpChoice.miCharacterId].macCharacterName + ":"  );						// Show character Id Questions..
					 
				miChatDirection = lcpChoice.miCharacterId % 2;
                
                for (var liOptionIndex : int = 0; liOptionIndex < (mpCurrentConversationNode.mChildren.Count );  liOptionIndex++ )
                {
                    var lcpOptions : cConversationNode  = ( mpCurrentConversationNode.mChildren[ liOptionIndex ]);//Show posible options

                    if( lcpOptions != lcpChoice)// Printing the unselected Options
                    {
						GUI.color = Color(.322, .306, .631, 1);
                    	GUI.Label (  Rect( (Screen.width * .5f)-200 , (Screen.height * .15f)+ liOptionIndex * 20, 486, 200), lcpOptions.macText );
                    }
                    else	if( lcpOptions == lcpChoice) // Printing the Choice
                    {
						GUI.color = Color.magenta;
//						GUI.color = Color(1, 0.36, 0.22, 1);
						
                    	GUI.Label (  Rect( (Screen.width * .5f)-200 , (Screen.height * .15f) + liOptionIndex * 20, 486, 200),  lcpOptions.macText );
                    }
                }
            	break;
        }
    }
 }
        //////////////////////////////////////////////////////////////
               
 	public function IsInConversation() : boolean 
 {
	  return (mbConversationState) ; 
 } 
        //////////////////////////////////////////////////////////////
};



//
//
//	public function Render() 
//	{
//	 if (mbConversationState && mpCurrentConversationNode != null )// Si Hay Conversación y el nodo no es NULL
//        {
////        cGraphicManager::Get().SetColor(eLightGreen, eBlack);// Don´t get Horrorized about this !!! it´s just the Dialog Window Frame XP
////        cGraphicManager::Get().WriteChars( 4, 6, "\xda\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xbf" );
////        cGraphicManager::Get().WriteChars( 4, 7, "\xb3                                                                      \xb3" );
////        cGraphicManager::Get().WriteChars( 4, 8, "\xc3\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xb4" );
////        cGraphicManager::Get().WriteChars( 4, 9, "\xb3                                                                      \xb3" );
////        cGraphicManager::Get().WriteChars( 4, 10, "\xb3                                                                      \xb3" );
////        cGraphicManager::Get().WriteChars( 4, 11, "\xb3                                                                      \xb3" );
////        cGraphicManager::Get().WriteChars( 4, 12, "\xb3                                                                      \xb3" );
////        cGraphicManager::Get().WriteChars( 4, 13, "\xc0\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xc4\xd9" ); 
////        cGraphicManager::Get().SetColor(eWhite, eBlack);
//                
//                switch ( mpCurrentConversationNode.meType)
//                {
//                    case eConversationNodeType.eNormalTalk: {	// Enum 0; Nodo activo de tipo eNormalTalk
//                                        
//                        	for (int liSpeakersId = 0; liSpeakersId < (int)mSpeakers.size(); ++liSpeakersId)
//                            { 
//                                    if (mSpeakers[liSpeakersId].miCharacterId == mpCurrentConversationNode->miCharacterId)
//                                            cGraphicManager::Get().WriteChars( 6, 7, mSpeakers[liSpeakersId].macCharacterName.c_str() );
//                            }       // Check If the character Id it´s the same as the Speakers Id and Show who is Speaking...
//                                            
//                            cGraphicManager::Get().WriteChars( 6, 9, mpCurrentConversationNode->macText.c_str() );// Character Speak
//                        } break;
//                        
//                   	case eConversationNodeType.eChooseTalk: {	// Enum 1; Nodo activo de tipo eChooseTalk
//
//                            cConversationNode * lcpChoice  = &( mpCurrentConversationNode->mChildren[ muiChooseOption ]);// Choosen option
//
//                            for (int liSpeakersId = 0; liSpeakersId < (int)mSpeakers.size(); ++liSpeakersId)
//                                {
//                                        if (mSpeakers[liSpeakersId].miCharacterId == lcpChoice->miCharacterId)
//                                                cGraphicManager::Get().WriteChars( 6, 7, mSpeakers[liSpeakersId].macCharacterName.c_str() );
//                                }       // Check If the character Id it´s the same as the Speakers Id and Show who is Asking...
//
//                            for (unsigned liOptionIndex = 0; liOptionIndex < (mpCurrentConversationNode->mChildren.size() ); liOptionIndex++ )
//                            {
//                                cConversationNode * lcpOptions  = &( mpCurrentConversationNode->mChildren[ liOptionIndex ]);//Show posible options
//
//                                if( lcpOptions != lcpChoice)// Printing the unselected Options
//                                {
//                                	cGraphicManager::Get().SetColor(eGray, eBlack);// Color de las opciones
//         	                    	cGraphicManager::Get().WriteChars( 6, (9 + liOptionIndex), lcpOptions->macText.c_str() );
//                                }
//                                else	if( lcpOptions == lcpChoice) // Printing the Choice
//                                {
//                                    cGraphicManager::Get().SetColor(eLightWhite, eBlack);// Color de la Respuesta elegida
//                                    cGraphicManager::Get().WriteChars( 6, (9 + liOptionIndex), lcpChoice->macText.c_str() );
//                                }
//                            }
//                        } break;
//                }
//        }
//	}
//
//       