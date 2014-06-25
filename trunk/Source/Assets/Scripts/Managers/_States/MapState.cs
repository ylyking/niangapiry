using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapState : GameState		
{
    uint ChooseOption = 2;
    uint TotalOptions = 0;
    Dictionary<int, MapNode> OptionsList = new Dictionary<int, MapNode>();

    public AudioClip Select;
    public AudioClip Play;
	public AudioClip MusicMap;

    float timeLapse = 0;
    bool MapSelected = false;
	

    GameObject Target;
    AnimSprite MiniAnim;
    GUISkin gSkin;
	
    public override void Init()
	{
        Debug.Log("Enter map State");

        if (Managers.Tiled.Load("Map") != true)
        {
            Debug.Log(" Map not loaded");
            Managers.Game.ChangeState(typeof(MainMenuState));
        }
		
//		Managers.Audio.PlayMusic( MusicMap, 1, 1);


        Target = GameObject.Find("mapindex");
        if (Target)
            MiniAnim = Target.GetComponent<AnimSprite>();

        OptionsList.Add(0, new MapNode( "Pampero"           , 2, 5, 4, 1,
            Target.transform.position + new Vector3(-1.5f, -2, 0)));
        OptionsList.Add(1, new MapNode( "Monte"             , 2, 5, 0, 3,
            Target.transform.position + new Vector3(1.5f, -2, 0)));
        OptionsList.Add(2, new MapNode( "Tatakua"              , 5, 1, 4, 3,
            Target.transform.position + (Vector3.forward * 0)));
        OptionsList.Add(3, new MapNode( "Iguazu"            , 5, 1, 2, 4 ,
            Target.transform.position + new Vector3(2, .6f, 0)));
        OptionsList.Add(4, new MapNode( "Campo Del Cielo"   , 5, 0, 3, 2,
            Target.transform.position + new Vector3(-2, .6f, 0)));
        OptionsList.Add(5, new MapNode( "Impenetrable"      , 0, 2, 4, 3,
            Target.transform.position + new Vector3(0, 2, 0)));

        gSkin = (GUISkin)Resources.Load("GUI/GUISkin B", typeof(GUISkin));

        //TotalOptions = (uint)OptionsList.Count;
        TotalOptions = (uint)Managers.Register.UnlockedStages;

        Managers.Display.cameraScroll.SetTarget(Target.transform);

        timeLapse = Time.time + 0.75f;    
        Target.GetComponent<CameraTargetAttributes>().distanceModifier = 0.05f;

        if (Managers.Register.FirstTimePlay)
        {
            Managers.Register.FirstTimePlay = false;
            Managers.Game.PushState(typeof(WorldState2));                   // Home World
        }

	}
	
    public override void DeInit()
	{
        //DestroyImmediate(Target);
        Managers.Tiled.Unload();
        Managers.Display.cameraScroll.SetTarget(Managers.Display.transform, false);
        Target = null;
        MiniAnim = null;

        OptionsList.Clear();
        ChooseOption = 2;               // Setting Home position
		
        Managers.Audio.StopMusic();
		
        //FullText = "";
        Debug.Log("Exit the current State and returning main menu");

	}
	
    public override void OnUpdate()
	{
        if (timeLapse > Time.time)
            Target.GetComponent<CameraTargetAttributes>().distanceModifier = 0.05f;
        else
            Target.GetComponent<CameraTargetAttributes>().distanceModifier = 2;

            if (Target != null)                                                                 // Pombe Animation
            {
                Target.transform.position = Vector3.Lerp(Target.transform.position, OptionsList[(int)ChooseOption].Position, Time.deltaTime * 5);
                
                if ( Mathf.Abs(Target.transform.position.x - OptionsList[(int)ChooseOption].Position.x) < 0.01f )
                    MiniAnim.PlayFrames(4, 4, 1, (int)Mathf.Sign(OptionsList[(int)ChooseOption].Position.x - Target.transform.position.x));
                else
                    MiniAnim.PlayFrames(4, 4, 3, (int)Mathf.Sign(OptionsList[(int)ChooseOption].Position.x - Target.transform.position.x));
            }

            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetButtonDown("Start"))
            {
                timeLapse = Time.time + 0.75f;
                Managers.Audio.Play(Play, Managers.Display.camTransform);
                MapSelected = true;
            }

            if (MapSelected)
            {
                if (timeLapse > Time.time)
                    return;
                Managers.Display.ShowFlash(1);

                MapSelected = false;
                switch (ChooseOption)
                {
                    case 0:
                        Managers.Game.PushState(typeof(WorldState0));                   // Pampero World
                        break;
                    case 1:
                        Managers.Game.PushState(typeof(WorldState1));                   // Monte World
                        break;
                    case 2:
                        Managers.Game.PushState(typeof(WorldState2));                   // Home World
                        break;
                    case 3:
                        Managers.Game.PushState(typeof(DemoAdvise));                   // Iguazu World
//                        Managers.Game.PushState(typeof(WorldState3));                   // Iguazu World
                        break;
                    case 4:
                        Managers.Game.PushState(typeof(DemoAdvise));                   // Iguazu World
//                        Managers.Game.PushState(typeof(WorldState4));                   // SkyField World       
                        break;
                    case 5:
                        Managers.Game.PushState(typeof(DemoAdvise));                   // Iguazu World
//                        Managers.Game.PushState(typeof(WorldState5));                   // Impenetrable World
                        break;
                }
            }
            else
            {

                #region Player Input Navigation

                if (Input.GetKeyDown("escape") || Input.GetButtonDown("Fire3") || Input.GetButtonDown("Select"))
                    Managers.Game.ChangeState(typeof(MainMenuState));

                //if (Input.GetKeyDown("up"))
                if (Managers.Game.InputUp)
                {
                    if (ChooseOption == TotalOptions - 1)
                        ChooseOption = 0;
                    else if (OptionsList[(int)ChooseOption].Up > TotalOptions - 1)
                        ChooseOption = TotalOptions - 1;
                    else
                        ChooseOption = OptionsList[(int)ChooseOption].Up;
                    Managers.Audio.Play(Select, Managers.Display.camTransform);
                }

                if (Managers.Game.InputDown)
                {
                    if (ChooseOption == 0 || (OptionsList[(int)ChooseOption].Down > TotalOptions - 1))
                        ChooseOption = TotalOptions - 1;
                    else
                        ChooseOption = OptionsList[(int)ChooseOption].Down;
                    Managers.Audio.Play(Select, Managers.Display.camTransform);
                }

                if (Managers.Game.InputLeft)
                {
                    if (OptionsList[(int)ChooseOption].Left > TotalOptions - 1)
                        ChooseOption = 0;
                    else
                        ChooseOption = OptionsList[(int)ChooseOption].Left;
                    Managers.Audio.Play(Select, Managers.Display.camTransform);
                }

                if (Managers.Game.InputRight)
                {
                    if (OptionsList[(int)ChooseOption].Right > TotalOptions - 1)
                        ChooseOption = 1;
                    else
                        ChooseOption = OptionsList[(int)ChooseOption].Right;
                    Managers.Audio.Play(Select, Managers.Display.camTransform);
                }

                #endregion

            }
	}

    public override void OnRender()
    {

        if (gSkin)
        {
            GUI.skin = gSkin;
            GUI.skin.label.fontSize = 32;
            GUI.color = new Color(1, 0.36f, 0.22f, 1);
        }
        else Debug.Log("MainMenuGUI : GUI skin object missing!");

        string FullText = "Proximo Mundo: ";

        GUI.Label(new Rect((Screen.width * .05f), (Screen.height * .9f), 400, 200), FullText + OptionsList[(int)ChooseOption].Name);

        GUI.color = Color.white;
    }

	
	public override void Pause()
	{
        Managers.Display.ShowFlash(1);
        Managers.Tiled.Unload();
        Managers.Display.cameraScroll.SetTarget(Managers.Display.transform, false);
        Target = null;
        MiniAnim = null;
	}
	
	public override void Resume()
	{
        if ( TotalOptions != (uint)Managers.Register.UnlockedStages)
            TotalOptions = (uint)Managers.Register.UnlockedStages;
		
//		Managers.Audio.PlayMusic( MusicMap, 1, 1);

        Managers.Display.ShowFlash(1);
        Managers.Tiled.Load("Map");
        Target = GameObject.Find("mapindex");
        MiniAnim = Target.GetComponent<AnimSprite>();
        Managers.Display.cameraScroll.SetTarget(Target.transform);
        Managers.Display.cameraScroll.ResetBounds( Managers.Display.cameraScroll.levelBounds);
        //Managers.Display.cameraScroll.ResetBounds();

        timeLapse = Time.time + 1;
        Target.GetComponent<CameraTargetAttributes>().distanceModifier = 0.05f;
	}

	
}

struct MapNode
{
    public string Name;
    public uint  Up;
    public uint  Down;
    public uint  Left;
    public uint  Right;
    public Vector3 Position;

    public MapNode(string name, uint up, uint down, uint left, uint right, Vector3 position)
    {
        this.Name = name;
        this.Up = up;
        this.Down = down;
        this.Left = left;
        this.Right = right;
        this.Position = position;
    }
}



//public class MapState : GameState		
//{


//    uint ChooseOption = 0;
//    uint TotalOptions = 0;
//    Dictionary<int, string> OptionsList = new Dictionary<int, string>();
//    //Dictionary<int, MapNode> NodeList = new Dictionary<int, MapNode>();
//    string FullText;
	
//    public override void Init()
//    {
//        Debug.Log("Enter map State");

//        OptionsList.Add(0, "Home");
//        OptionsList.Add(1, "Monte");
//        OptionsList.Add(2, "Pampero");
//        OptionsList.Add(3, "Iguazú");
//        OptionsList.Add(4, "SkyField");
//        OptionsList.Add(5, "Impenetrable");
//        TotalOptions = (uint)OptionsList.Count;

//        foreach (int Option in OptionsList.Keys)
//            FullText += (OptionsList[Option] + System.Environment.NewLine); // This it's a fix to reduce the options drawcalls

//    }
	
//    public override void DeInit()
//    {
//        OptionsList.Clear();
//        FullText = "";
//        Debug.Log("Exit the current State and returning main menu");

//    }
	
//    public override void OnUpdate()
//    {
//        //Debug.DrawLine(Managers.Display.MainCamera.transform.position,
//        //    Managers.Display.MainCamera.transform.position + new Vector3( 0, 2, 2), Color.red);
//        //Debug.DrawLine(Managers.Display.MainCamera.transform.position,
//        //    Managers.Display.MainCamera.transform.position + new Vector3( 2, .6f, 2) , Color.red);
//        //Debug.DrawLine(Managers.Display.MainCamera.transform.position,
//        //    Managers.Display.MainCamera.transform.position + new Vector3(-2, .6f, 2), Color.red);
//        //Debug.DrawLine(Managers.Display.MainCamera.transform.position,
//        //    Managers.Display.MainCamera.transform.position + new Vector3( 1.5f,-2, 2), Color.red);
//        //Debug.DrawLine(Managers.Display.MainCamera.transform.position,
//        //    Managers.Display.MainCamera.transform.position + new Vector3(-1.5f,-2, 2), Color.red);

//        //ChooseOption = (ChooseOption == 0 ? TotalOptions : ChooseOption - 1);


//        if ( Input.GetKeyDown("escape") )
//            Managers.Game.ChangeState(typeof(MainMenuState));

//        if (Input.GetKeyDown("up") && (ChooseOption > 0))
//            ChooseOption--;

//        if (Input.GetKeyDown("down") && (ChooseOption < (TotalOptions - 1)))
//            ChooseOption++;

//        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
//            switch (ChooseOption)
//            {
//                case 0:
//                    Managers.Display.ShowFlash(1);
//                    Managers.Game.PushState(typeof(WorldState0));
//                    break;
//                case 1:
//                    Managers.Display.ShowFlash(1);
//                    Managers.Game.PushState(typeof(WorldState1));
//                    break;
//                case 2:
//                    Managers.Display.ShowFlash(1);
//                    Managers.Game.PushState(typeof(WorldState2));
//                    break;
//                case 3:
//                    Managers.Display.ShowFlash(1);
//                    Managers.Game.PushState(typeof(WorldState3));
//                    break;
//                case 4:
//                    Managers.Display.ShowFlash(1);
//                    Managers.Game.PushState(typeof(WorldState4));
//                    break;
//                case 5:
//                    Managers.Display.ShowFlash(1);
//                    Managers.Game.PushState(typeof(WorldState5));
//                    break;
//            }
//    }

//    public override void OnRender()
//    {
//        //if (gSkin)
//        //{
//        //    GUI.skin = gSkin;
//        //    GUI.skin.label.fontSize = 32;
//        //    GUI.color = new Color(1, 0.36f, 0.22f, 1);
//        //}
//        //else Debug.Log("MainMenuGUI : GUI skin object missing!");



//        GUI.color = new Color(1, 0.36f, 0.22f, 1);
//        GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .5f), 400, 200), FullText);
//        string jump = "";

//        foreach (int Option in OptionsList.Keys)
//        {

//            if (Option == ChooseOption)
//            {
//                GUI.color = Color.white;
//                GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .5f), 400, 200), jump + OptionsList[Option] );
//            }

//            jump += System.Environment.NewLine;
//        }

//        GUI.color = Color.white;
//    }

	
//    public override void Pause()
//    {
	
//    }
	
//    public override void Resume()
//    {
//        Managers.Display.ShowFlash(1);
//    }
	
//    //public override void CheckScore()
//    //{
////        string MissionScore = (Managers.Game.State.GetType()).ToString();						// Get current Mission Top Score name
////        int MissionId = int.Parse( MissionScore.Substring( MissionScore.LastIndexOf("_")+ 1 ) );	 
////        MissionScore = "Top Score " + MissionId   ; 
//////		Debug.Log("Loading" + MissionScore);
		
////        if( this.score > PlayerPrefs.GetInt( MissionScore ) )
////        {
////            PlayerPrefs.SetInt( MissionScore, (int)score );
////            GreatScore.text += this.score;
////            GreatScore.hidden = false;
////        }
//    //}
	
////	public virtual void ShowStatus()
////	{
////		PauseButton.hidden 	= false;
////		text1.hidden 		= false;
////		text2.hidden 		= false;
////		text3.hidden 		= false;
////	}
	
////	public virtual void LevelCompleted()
////	{
////		;
////	}	
////	
//}