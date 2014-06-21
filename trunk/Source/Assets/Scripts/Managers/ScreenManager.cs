using UnityEngine;
using System.Collections;


public class ScreenManager : MonoBehaviour
{
	public Camera MainCamera        ;
	public Transform camTransform;
	public CameraScrolling  cameraScroll ;   
	
	GUISkin   gSkin                 = null;
	GUISkin   gSkinB                = null;
	
	
	public string DebugText = "DebugText Ready!";
	public bool EnableDebug = false;
	
	#region Show Flash Transition
	
	public Texture2D FlashTex           = null;                          // Scene Rainbow tranition between scenes
	Rect FlashPos                   = new Rect(0, 0, Screen.width, Screen.height);
	bool FlashBang                  = false;
	
	float lastUpdate	            = 0;	
	float FlashLapse                = 0;
	
	int FlashX	 	                = 0;	
	int FlashY	 	                = 3;
	
	#endregion
	
	// ----------------------------------------
	
	#region Show InGame Status 
	
	public float ShowDelay          = 6.0f;                          // TimeLapse lapse to display Player status and then hide
	public bool ShowState           = false;
	float TimeLapse                 = 0.0f;
	
	Texture2D HealthTex             = null;
	Rect HealthPos                  = new Rect();
	//Rect HealthCoord                = new Rect(0, 0, .25f, .25f);               // player's Health HUD system
	Rect HealthCoord                = new Rect(0, 0, .125f, .125f);               // player's Health HUD system
	
	Texture2D LifesTex              = null;
	Rect LifesPos                   = new Rect();
	//Rect LifesCoord                 = new Rect(0, 0, 0.5f, 0.5f);               // player's Lifes HUD system
	Rect LifesCoord                 = new Rect(.5f, .125f, .125f, .125f);               // player's Lifes HUD system
	
	//HealthCoord = new Rect( Managers.Game.Health * .125f, 0.125f, .125f, .125f);
	
	#endregion
	
	// ----------------------------------------
	
	#region Show Fading Transition
	
	public Texture2D FadeTex		= null;
	float FadeSpeed                 = 0.5f;                                     // Fade In/Out Transition speed
	bool  Fading                    = false;                                    // Fading State Activator       
	int   FadeDir                   = -1;
	float currentAlpha              = 0; 
	Color alphaColor                = new Color();								// Color object for alpha setting
	
	#endregion
	
	// ----------------------------------------
	
	#region Show Image Display
	
	Texture2D ImageTex              = null;
	bool     ImageDisplay           = false;
	float    ImageLapse             = 0;
	
	#endregion
	
	// ----------------------------------------
	
	void Awake () 
	{
		if (MainCamera == null)
			Debug.Log("Warning, Main Camera not setup");
		else
		{
			camTransform = MainCamera.transform;
			cameraScroll = MainCamera.gameObject.GetComponent<CameraScrolling>();
		}
		
		//Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
		if ( FadeTex == null)
			FadeTex = Resources.Load("GUI/PixelBlack") as Texture2D;
		
		if ( FlashTex == null)
			FlashTex = Resources.Load("GUI/Rainbow") as Texture2D;
		gSkin    = Resources.Load("GUI/GUISkin") as GUISkin;
		
		gSkinB = Resources.Load("GUI/GUISkin B") as GUISkin;
		gSkinB.label.fontSize 	= Mathf.RoundToInt( Screen.width * 0.035f );
		
		//HealthTex = Resources.Load("GUI/Items") as Texture2D;
		//HealthPos = new Rect((Screen.width *.025f), (Screen.height *.75f), HealthTex.width *.65f, HealthTex.height *.65f);
		HealthTex = Resources.Load("GUI/Items") as Texture2D;
		HealthPos = new Rect((Screen.width *.025f), (Screen.height *.75f), HealthTex.width *.3f, HealthTex.height *.3f);
		
		
		//LifesTex = Resources.Load("GUI/Lifes") as Texture2D;
		//LifesPos = new Rect((Screen.width * .85f), (Screen.height * .85f), LifesTex.width, LifesTex.height);
		
		LifesTex = Resources.Load("GUI/Items") as Texture2D;
		LifesPos = new Rect((Screen.width * .825f), (Screen.height * .825f), LifesTex.width *.2f, LifesTex.height *.2f);
	}
	
	public void ShowStatus()
	{
		this.ShowState = true;
		TimeLapse = ShowDelay;
	}   
	
	public void ShowFlash(float FlashDelay)
	{
		if (FlashBang) return;
		
		FlashPos.width = Screen.width;
		FlashPos.height = Screen.height;
		
		FlashBang = true;
		FlashLapse = FlashDelay;
	}
	
	public bool ShowImage( string path, float lapse = 2)
	{
		
		//#if TEXTURE_RESOURCE
		ImageTex = (Texture2D)Resources.Load( path, typeof(Texture2D) )  ;
		//#else
		//    WWW www = new WWW("file://" + path);
		//    ImageTex = www.texture  ;
		//#endif
		
		
		if ( ImageTex != null )
		{
			ImageDisplay = true;
			ImageLapse = Time.time + lapse;
		}
		
		return ImageDisplay;
	}
	
	public bool ShowImage( Texture2D img, float lapse = 2)
	{
		ImageTex = img ;
		//
		//		if ( ImageTex != null )
		//		{
		ImageDisplay = true;
		ImageLapse = Time.time + lapse;
		//		}
		
		return ImageDisplay;
	}
	
	public void ShowFade( float speed = 2, bool fadeIn = true )           				// Usage in Ex: ShowFade(speed = 2, dir = 1)  FADE IN = TRUE        (DARKENING)              
	{																					// By Default: 	ShowFade(speed = 2, dir = -1) FADE OUT = FALSE
		//alphaColor.a = currentAlpha = 0.99f;
		FadeDir = (fadeIn ? 1 : -1);	
		FadeSpeed = speed;
		Fading  = true;
	}
	
	public float PaletteSwap = 0;
	public Vector2 WaterFlow;       // This both Are Updated 
	public Vector2 WaterFlowAlpha;   
	
	
	void Update()
	{
		PaletteSwap += Time.deltaTime * 4;
		PaletteSwap %= 1;
		//FireTime = Mathf.Repeat(FireTime, 1);
		
		//int index = ((int)(Time.time * 16)) % 2; // time control fps
		int u = (((int)(Time.time * 16)) % 2) % 8;
		WaterFlow = new Vector2(((u + 4) * .125f), .375f); // offset
		WaterFlowAlpha = new Vector2(((u) * .125f), .375f); // offset alpha
		
		
		if ( FlashBang )                                                        // FLASHBANG 
		{
			if( (Time.time - lastUpdate) >= 0.021f ) 							// secs to update increment: 1/12 (12 frames) my speed: 48 frames / 1 sec
			{ FlashX++; lastUpdate = Time.time; }
			
			FlashY -= System.Convert.ToByte( FlashX > 3 );
			
			FlashX = FlashX % 4;
			FlashY += 3 * System.Convert.ToByte( (FlashY == 0) ); 				//FlashY % 4 (!FlashY);
			
			FlashLapse -= Time.deltaTime;
			FlashBang = (FlashLapse > 0);
		}
		
		if ( ShowState  )                                                        // SHOW PLAYER STATUS
		{   
			TimeLapse -= Time.deltaTime;	                					 // Decrease the message time
			ShowState = (TimeLapse > 0);
		}
		
		//HealthCoord = new Rect( Managers.Game.Health * .25f, 0, .25f, .25f);
		HealthCoord = new Rect( Managers.Register.Health * .125f, 0.125f, .125f, .125f);
		
		if ( ImageDisplay )                                                     // Display some Image while lapse is bigger
			ImageDisplay = (Time.time < ImageLapse);
		
		if ( Fading )                                                           // FADE IN/OUT
		{   
			currentAlpha += FadeDir * FadeSpeed * Time.deltaTime;	
			currentAlpha = Mathf.Clamp01(currentAlpha);
			alphaColor.a = currentAlpha;
			Fading = !(currentAlpha == 1 || currentAlpha == 0); // "If the the alpha transition ended Then stop Fading"
		}
		
		if ( Input.GetKeyDown("f") )
			EnableDebug = !EnableDebug;
		
		//Managers.Display.DebugText =  " Time.time: " + Time.deltaTime + " Time.realTimeSinceStart: " + Time.fixedDeltaTime;
		
		
	}
	//////////////////////////////////////////////////////////////   
	
	void  OnGUI ()
	{
		//if ( !Managers.Game.IsPlaying ) return;
		
		if( Event.current.type == EventType.Repaint) 
		{
			if( Managers.Dialog.IsInConversation() )
				Managers.Dialog.Render();
			
			//Managers.Display.Render();
			
			Managers.Game.Render();
			
			Managers.Display.Render();
		}
	}
	
	
	public void Render()
	{
		//     FlashPos                   = new Rect(0, 0, Screen.width, Screen.height);
		//		if (FlashPos.
		
		if (FlashBang && FlashTex)
		{
			GUI.DrawTextureWithTexCoords(FlashPos, FlashTex, new Rect(FlashX * .25f, FlashY * .25f, .25f, .25f));
			return;
		}
		
		if (gSkin) GUI.skin = gSkin;
		
		if ( ImageDisplay )
		{
			GUI.depth = -1000;
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), ImageTex);
		}
		
		// ------------------------------------------------ FADE IN OUT
		
		if ( currentAlpha > 0  && FlashTex )                                       // Draw only if not transculent
		{
			GUI.color = alphaColor;
			//            GUI.DrawTextureWithTexCoords(FlashPos, FlashTex, new Rect(.5f, 0, .25f, .25f));
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), FadeTex);
		}
		
		if (gSkinB) GUI.skin = gSkinB;
		
		if (Managers.Game.IsPlaying)
		{
			GUI.DrawTextureWithTexCoords(HealthPos, HealthTex, HealthCoord);                    // Health Bar HUD
			
			if (!ShowState) return;
			
			gSkinB.label.fontSize = Mathf.RoundToInt(Screen.width * 0.035f);
			
			GUI.DrawTextureWithTexCoords(LifesPos, LifesTex, LifesCoord);                       // Life remaining HUD
			
			GUI.color = Color.magenta;
			GUI.Label( new Rect((Screen.width * .05f), (Screen.height * .02f), 100, 50),
			          "Score: " + Managers.Register.Score + "\n" + "Fruits: " + Managers.Register.Fruits);
			GUI.Label(new Rect((Screen.width * .92f), (Screen.height * .9f), 200, 50), "x" + Managers.Register.Lifes); // Score
		}
		
		if ( EnableDebug )
		{
			//GUI.color = Color.black;
			GUI.skin.label.fontSize = 14;
			GUI.Label(new Rect((Screen.width * .25f), (Screen.height * .9f), 600, 200), DebugText);
		}
	}
}