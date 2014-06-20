
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
	Rect ComicWord;
	
	int miChatDirection = 0;														                    //  indicates the current Dialog's direction of Comic balloon 
	int miCharIndex = 0;														                        //  indicates the current Dialog's char Index of script Rendering
	int miChooseIndex = 0;
	
	List<cCharacterSpeaker> mSpeakers = new List<cCharacterSpeaker>();                                  // Characters talk able list
	List<cConversation> mConversations = new List<cConversation>();		                                // Conversations list
	cConversationNode mpCurrentConversationNode;
	cConversationNode mpPreviousConversationNode = null;
	
	bool  mbConversationState = false;
	float mfMessageTime;
	uint muiChooseOption = 0;														                    // Actual choice
	public int CharShowDelay = 20;
	CameraTargetAttributes CamTarget = null;
	
	//	cConversation cConversationIt;
	//	cConversation cCIterator	;
	
	public void  Init (  TextAsset lacConversationFile   ){
		gSkin = Resources.Load ("GUI/GUISkin") as GUISkin;
		ComicTex = Resources.Load("GUI/Rainbow") as Texture2D;
		//ComicPos = new Rect( ( Screen.width * .24f), (Screen.height * .05f), ComicTex.width *1.02f , ComicTex.height *.5f);
		ComicPos = new Rect((Screen.width * .24f), (Screen.height * .05f), 
		                    (Screen.width * .76f) - (Screen.width * .24f),
		                    (Screen.height * .45f) - (Screen.height * .05f) );
		ComicCoord = new Rect( 0, 0, .5f, .25f);
		
		ComicWord = new Rect((Screen.width * .27f), (Screen.height * .1f),
		                     (Screen.width * .46f), (Screen.height * .45f) ); 
		
		XmlDocument Doc = new XmlDocument();
		Doc.LoadXml(lacConversationFile.text);
		
		
		if ( Doc != null && (Doc.DocumentElement.Name == "Conversations") )
		{
            foreach (XmlNode CharacterInfo in Doc.GetElementsByTagName("Character"))	                        // Read all the Characters        
			{
                this.ParseCharacter(CharacterInfo);                                                             // array of the level nodes.
			}

            foreach (XmlNode ConversationInfo in Doc.GetElementsByTagName("Conversation"))	                    // Read all the conversations             
			{
                ParseConversation(ConversationInfo);                                                            // array of the level nodes.   			
			}
			
			mpCurrentConversationNode = null;
			muiChooseOption = 0;
		}   	
	}       
	//////////////////////////////////////////////////////////////
	
	public void DeInit()
	{ StopConversation(); mSpeakers.Clear(); mConversations.Clear(); mpCurrentConversationNode = null; }
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
		lConversation.macName = lpElem.Attributes["nameId"].Value ;                                             // guardando el nombre de la conversación
		
		// Read the tree
		ParseNode( lpElem.FirstChild, lConversation.mRoot  );                                                   // guardando la ruta de la conversación
		
		// Add the conversations to the list
		mConversations.Add( lConversation );                                                                    // guardando en la lista la conversación;
		
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
 
	public void  StartConversation ( string lacConversationId   )
	{
		mbConversationState = true; 									                                    // stops Player moves during Conversation
		
		foreach( cConversation lpIterator in mConversations )
		{       
			if (lpIterator.macName == lacConversationId )				                                    // If we have a talking with the correct Character Id...
			{   
				mpCurrentConversationNode = lpIterator.mRoot; 			                                    // Get access Root
				
				mfMessageTime = mpCurrentConversationNode.mfDuration; 	                                    // Asignamos el tipo inicial
				
				muiChooseOption = 0;
				
				mpPreviousConversationNode = null;
			}
		}
		
		//StartCoroutine(CoUpdate(TimeLapse.deltaTime));
		
	} // con esto llamamos al Conversation Manager desde Statement.cpp
	//////////////////////////////////////////////////////////////

	public void Update() //: IEnumerator
	{
		if (CamTarget == null && Managers.Game.PlayerPrefab)
			CamTarget = Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>() as CameraTargetAttributes; 
		
		if (IsInConversation())
		{
			//if (!mbConversationState) return;
			
			if (CamTarget.Offset.x > 0)                                                                     // Check Player Position and Id
				miChatDirection = System.Convert.ToByte(miChatDirection != 0);                              // the Player is on left side
			else
				miChatDirection = System.Convert.ToByte(miChatDirection == 0);                              // else the Player is on right 
			
			
			ComicCoord = new Rect(.5f * miChatDirection, 0, (miChatDirection > 0 ? -.5f : .5f), .25f);
			
			
			if (!Managers.Game.IsPaused)
				if (mpCurrentConversationNode != null)
			{
				switch (mpCurrentConversationNode.meType)
				{
				case eConversationNodeType.eNormalTalk:									                    // Enum 0; Nodo activo de tipo eNormalTalk
					
					if ( mpCurrentConversationNode != mpPreviousConversationNode )
					{

						miCharIndex += System.Convert.ToByte(Time.frameCount % CharShowDelay == 0);			// Increment each 10 Secs of Frames (speed)
						
						if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetKeyDown("return"))
						{
							miCharIndex = 0;
							mpPreviousConversationNode = mpCurrentConversationNode;
						}
						break;
					}
					
					mfMessageTime -= Time.deltaTime * .1f;	                					            // Decrease the message time
					
					if ((mfMessageTime <= 0.0f) || Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
						NextMessage(0);               								                        //Need to continue with the next message or node
					break;
					
				case eConversationNodeType.eChooseTalk:									                    // Enum 1; Nodo activo de tipo eChooseTalk
					
					if (Managers.Game.InputUp && (muiChooseOption > 0))
						muiChooseOption--;

					if (Managers.Game.InputDown && (muiChooseOption < (mpCurrentConversationNode.mChildren.Count - 1)))
						muiChooseOption++;

					if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") || Input.GetKeyDown("return"))
					{
						if ( miChooseIndex < mpCurrentConversationNode.mChildren.Count )
						{
							miCharIndex = 0;
							miChooseIndex =  mpCurrentConversationNode.mChildren.Count;
							break;
						}

						miCharIndex = 0;
						miChooseIndex = 0;
						mpPreviousConversationNode = mpCurrentConversationNode;
						NextMessage(muiChooseOption);
					}
					break;
					
				default:																                    // Enum 2; Nodo activo de tipo eEndConversation
					mpCurrentConversationNode = null;
					break;
				}
			}
			else mbConversationState = false; // If we have a null mpCurrentConversationNode then it´s the end
			
			
		}
	}
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
			case eConversationNodeType.eNormalTalk: 													    // Enum 0; Nodo activo de tipo eNormalTalk
				
				GUI.Label(new Rect(ComicWord.xMin, (Screen.height * .1f), ComicWord.width  , 200),
				          mSpeakers[mpCurrentConversationNode.miCharacterId].macCharacterName + " :"  );		// Show character Id Dialog
				
				//miChatDirection = (mpCurrentConversationNode.miCharacterId == 1 ? 1 : 0);
				miChatDirection = System.Convert.ToByte(mpCurrentConversationNode.miCharacterId == 1 );
				
				
				if ( mpCurrentConversationNode != mpPreviousConversationNode )
				{
					
					if ( miCharIndex < mpCurrentConversationNode.macText.Length)
					{
						GUI.Label( new Rect(ComicWord.xMin, (Screen.height * .15f), ComicWord.width, 200), 
						          (mpCurrentConversationNode.macText).Remove(miCharIndex).ToString());      // Character Speak	
						break;
					}	
					else 
					{
						miCharIndex = 0;
						mpPreviousConversationNode = mpCurrentConversationNode;
					}	

				}
				
				GUI.Label(new Rect(ComicWord.xMin, (Screen.height * .15f), ComicWord.width, 200), mpCurrentConversationNode.macText);// Character Speak														///*///
				break;
				
			case eConversationNodeType.eChooseTalk: 													    // Enum 1; Nodo activo de tipo eChooseTalk
				
				cConversationNode lcpChoice = (mpCurrentConversationNode.mChildren[(int) muiChooseOption ]) as cConversationNode;// Choosen option
				
				GUI.Label(new Rect(ComicWord.xMin, (Screen.height * .1f), ComicWord.width , 200),
				          mSpeakers[lcpChoice.miCharacterId].macCharacterName + " :"  );					// Show character Id Questions..
				
				miChatDirection = System.Convert.ToByte(lcpChoice.miCharacterId == 1);                      // Orientate Comic Balloon 
				
				
				for (int liOptionIndex = 0, liOptionTotal = mpCurrentConversationNode.mChildren.Count;	liOptionIndex < liOptionTotal;	liOptionIndex++ )
				{
				
					cConversationNode lcpOptions  = ( mpCurrentConversationNode.mChildren[ liOptionIndex ]);//Show posible options

					if( lcpOptions != lcpChoice)                                                            // Printing the unselected Options
					{
						GUI.color = new Color(.322f, .306f, .631f, 1);
					}
					else	if( lcpOptions == lcpChoice)                                                    // Printing the Choice with enhaced color
					{
						if (((int)(Time.time * 5)) % 2 == 0)
							GUI.color = new Color(1, 0.36f, 0.75f, 1);
						else 
							GUI.color = Color.magenta; 														//  GUI.color = Color(1, 0.36f, 0.22f, 1);
					}

					if( miChooseIndex == liOptionTotal || miChooseIndex > liOptionIndex )					// if Traversed Options are equal than total or lesser than current one...
						GUI.Label( new Rect(ComicWord.xMin, (Screen.height * .15f) + liOptionIndex * 40, ComicWord.width , 200), lcpOptions.macText);// Character Speak	

					else if ( miChooseIndex == liOptionIndex )												// else do typeWriter machine FX ...
						{
							GUI.Label(new Rect(ComicWord.xMin, (Screen.height * .15f) + liOptionIndex * 40, ComicWord.width , 200), 
							          (lcpOptions.macText).Remove(miCharIndex).ToString());	
							
							miCharIndex += System.Convert.ToByte(Time.frameCount % CharShowDelay == 0);		// Increment each 10 Secs of Frames (speed)
							
						if ( miCharIndex >= lcpOptions.macText.Length-1)									// Do it line by line
							{
								miCharIndex = 0;
								miChooseIndex++;
							}
						}
					///*///																					// else don't draw nothing
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
		mfMessageTime = 0;
		muiChooseOption = 0;
		mbConversationState = false;
		//StopCoroutine("CoUpdate");
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