using UnityEngine;
using System.Collections;


public class ScreenManager : MonoBehaviour
{
	
//	public int Score;
	public bool HUDCorned = true;
    private bool ShowState = false;

    float ShowDelay     = 6.0f;
    float TimeLapse     = 0.0f;
    float FlashLapse    = 0.0f;

    Texture2D FlashTex = null;
    Rect FlashPos       = new Rect(0, 0, Screen.width, Screen.height);
    bool FlashBang      = false;

	float lastUpdate	= 0.0f;	
	int FlashX	 	    = 0;	
	int FlashY	 	    = 3;
	
	void Awake () 
	{
        //Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
        FlashTex = Resources.Load("ñGUI/Rainbow") as Texture2D;

	}

    public void ShowStatus()
    {
        this.ShowState = true;
        TimeLapse = ShowDelay;
    }   

    public void ShowFlash(float FlashDelay)
    {
        if (FlashBang) return;
        FlashBang = true;
        FlashLapse = FlashDelay;
    }

    //public void Update(float lfTimestep)
    public void OnUpdate(float lfTimestep)
    {
    	if ( FlashBang )
    	{
			if( (Time.time - lastUpdate) >= 0.021f ) // secs to update increment: 1/12 (12 frames) my speed: 48 frames / 1 sec
				{ FlashX++; lastUpdate = Time.time; }
			
			FlashY -= System.Convert.ToByte( FlashX > 3 );
			
			FlashX = FlashX % 4;
			FlashY += 3 * System.Convert.ToByte( (FlashY == 0) ); //FlashY % 4 (!FlashY);
			
    		FlashLapse -= lfTimestep;
    		FlashBang = (FlashLapse > 0.0f);
    	}
    	
    	if ( ShowState  )
    	{
    		TimeLapse -= lfTimestep;	                					// Decrease the message time
            ShowState = (TimeLapse > 0.0f);
        }
    }
    //////////////////////////////////////////////////////////////     

    public void Render()
    {
        if (FlashBang && FlashTex)
        {
            GUI.DrawTextureWithTexCoords(FlashPos, FlashTex, new Rect(FlashX * .25f, FlashY * .25f, .25f, .25f));
            return;
        }

        //if (gSkin) GUI.skin = gSkin;


        //if (IsPaused)
        //{
        //    GUI.color = Color(1, 0.36f, 0.22f, 1);
        //    GUI.Box(new Rect((Screen.width * .5f) - (Screen.width * .15f),
        //                 (Screen.height * .5f) - (Screen.height * .15f),
        //                  (Screen.width * .3f), (Screen.height * .3f)),
        //                    "\n\n - PAUSE - \n press 'Q' to Quit Game \n and return Main Menu ");
        //    GUI.color = Color.clear;
        //    return;
        //}

        //if (IsPlaying)
        //{
        //    GUI.DrawTextureWithTexCoords(HealthPos, HealthTex, HealthCoord);

        //    if (!ShowState) return;

        //    GUI.DrawTextureWithTexCoords(LifesPos, LifesTex, LifesCoord);

        //    GUI.color = Color.magenta;
        //    GUI.Label(new Rect((Screen.width * .05f), (Screen.height * .02f), 100, 50),
        //         "Score: " + Score + "\n" + "Fruits: " + Fruits);
        //    GUI.Label(new Rect((Screen.width * .92f), (Screen.height * .9f), 200, 50), "x" + Lifes);
        //}

        //if (Lifes <= 0)
        //{
        //    //		    	GUI.skin.label.fontSize = 64;
        //    //		    	GUI.skin.label.fontStyle = FontStyle.Bold;
        //    GUI.color = Color.magenta;
        //    GUI.Label(new Rect((Screen.width * .35f), (Screen.height * .5f), 100, 50), "- GAME OVER -");
        //}

    }
	


	
	// Update is called once per frame
//	void Update () {
//	
//	}
	
//	void OnGUI()
//	{
//		GUI.Label( new Rect(Screen.width*.5f, Screen.height*.5f, 200, 200), "Hello World");
//	}
}
