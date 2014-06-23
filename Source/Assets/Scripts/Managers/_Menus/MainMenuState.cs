using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuState : GameState 
{
    public bool isLoading = false;
    public bool DisplayMenu = false;
    public bool DisplayInfo = false;
    public bool MusicIntro = true;

    public float timeTrailer = 30;

    uint ChooseOption = 0;
    uint TotalOptions = 0;
    Dictionary<int, string>  OptionsList = new Dictionary<int, string>();

    string FullText = "";

    public AudioClip PreOpening;
    public AudioClip Opening;
    GameObject IntroPrefab;
    GUISkin gSkin;

    Camera camBack;
    Camera camMiddle;
    Camera camFront;
    Camera MainCam;

    Transform SkyTransform;
    Transform CamTransform;

    public override void Init()
    {
        timeTrailer = 30;

        Managers.Audio.Play(PreOpening, Managers.Display.MainCamera.transform, 1, 1);

        gSkin = (GUISkin)Resources.Load("GUI/GUISkin B", typeof(GUISkin));
        IntroPrefab = (GameObject)Instantiate(Resources.Load("Prefabs/Intro/IntroPrefab", typeof(GameObject)) );

        Managers.Display.MainCamera.enabled = false;
        Managers.Display.MainCamera.tag = "Untagged";

        camBack = GameObject.Find("Camera_Back").GetComponent<Camera>() as Camera;
        camMiddle = GameObject.Find("Camera_Middle").GetComponent<Camera>() as Camera;
        camFront = GameObject.Find("Camera_Front").GetComponent<Camera>() as Camera;

        MainCam = GameObject.Find("Camera_Ortho").GetComponent<Camera>() as Camera;
        MainCam.tag = "MainCamera";
        CamTransform = MainCam.transform;
        SkyTransform = GameObject.Find("Intro_Clouds").transform;


        if (camFront)
        {
            MainCam.orthographicSize = 0.01f;
            camFront.fieldOfView = 170;
			camMiddle.fieldOfView = 170;
			camBack.fieldOfView = 170;
        }

        OptionsList.Add(0, "Comenzar");
        OptionsList.Add(1, "Opciones");
        OptionsList.Add(2, "Salir");
        //OptionsList.Add(0, "Comenzar");
        //OptionsList.Add(1, "Records");
        //OptionsList.Add(2, "Opciones");
        //OptionsList.Add(3, "Salir");
        TotalOptions = (uint)OptionsList.Count;

        foreach (int Option in OptionsList.Keys)
            FullText += (OptionsList[Option] + System.Environment.NewLine); // This it's a fix to reduce the options drawcalls

        //Managers.Game.TopScore = PlayerPrefs.GetInt("TopScore");
    }
	
    public override void DeInit()
    {
        Managers.Display.MainCamera.enabled = true;
        Managers.Display.MainCamera.tag = "MainCamera";

        DestroyImmediate(IntroPrefab);
        IntroPrefab = null;
        camBack = null;
        camMiddle = null;
        camFront = null;
        MainCam = null;
        CamTransform = null;
        OptionsList.Clear();

        FullText = "";
        Managers.Audio.StopMusic();

        //PlayerPrefs.SetInt("TopScore", (int)Managers.Game.TopScore);

    }

    public override void OnUpdate()
    {
  
		if (Input.anyKeyDown)
        	timeTrailer = 10;

        timeTrailer -= Time.deltaTime;

        if (timeTrailer <= 0)
        {
            Managers.Game.ChangeState(typeof(TrailerState));
            return;
        }

        CamTransform.position = new Vector3((float)System.Math.Round((Mathf.Sin(Time.time) * 0.05f), 4),
                                            CamTransform.position.y, CamTransform.position.z); // OJo aca

        if (SkyTransform)
            SkyTransform.Rotate(Vector3.forward * Time.deltaTime * 10);

		DisplayMenu = (camFront.fieldOfView <= 58);


        if (MainCam.orthographicSize < 0.9f)
            MainCam.orthographicSize += Time.deltaTime;

        if (!DisplayMenu)
        {
            float speed = 40;
            if (Input.anyKey)
                speed = 100;

			camBack.fieldOfView -= Time.deltaTime * speed;
			camMiddle.fieldOfView -= Time.deltaTime * speed;
			camFront.fieldOfView -= Time.deltaTime * speed;
            return;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        //if (!DisplayMenu)
        //    return;

        if (Managers.Game.InputUp && (ChooseOption > 0) && !DisplayInfo)
            ChooseOption--;
        if (Managers.Game.InputDown && (ChooseOption < (TotalOptions - 1)) && !DisplayInfo)
            ChooseOption++;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetButtonDown("Start"))
            switch (ChooseOption)
            {
                case 0:
                    Managers.Display.ShowFlash(1);
                    Managers.Game.ChangeState(typeof(MapState));
                    break;
                case 1:
                    Managers.Display.ShowFlash(.25f);
                    Managers.Game.PushState(typeof(ConfigMenuState));
                    break;
                case 2:
                    Managers.Display.ShowFlash(1);
                    Application.Quit();
                    break;
                //case 0:
                //    Managers.Display.ShowFlash(1);
                //    Managers.Game.ChangeState(typeof(MapState));
                //    break;
                //case 1:
                //    Managers.Display.ShowFlash(.25f);
                //    Managers.Game.PushState(typeof(TopScores));
                //    break;
                //case 2:
                //    Managers.Display.ShowFlash(.25f);
                //    Managers.Game.PushState(typeof(ConfigMenuState));
                //    break;
                //case 3:
                //    Managers.Display.ShowFlash(1);
                //    Application.Quit();
                //    break;
            }
    }

    public override void OnRender()
    {
        if (!DisplayMenu)
            return;

        if (Opening && !Managers.Audio.Music.isPlaying && Managers.Audio.SoundEnable && MusicIntro)
            Managers.Audio.PlayMusic(Opening, 1, 1);

        if (gSkin)
        {
            GUI.skin = gSkin;
            GUI.skin.label.fontSize = 32;
            GUI.color = new Color(1, 0.36f, 0.22f, 1);
        }
        else Debug.Log("MainMenuGUI : GUI skin object missing!");
		
        GUI.Label(new Rect((Screen.width * .01f), (Screen.height * .875f), 400, 200), "DEMO"  );
		
        if (isLoading)
        {
            GUI.Label(new Rect((Screen.width * .5f) - 100, (Screen.height * .5f), 400, 50), "Loading...");
            return;
        }


        GUI.color = new Color(1, 0.36f, 0.22f, 1);
        GUI.Label(new Rect((Screen.width * .01f), (Screen.height * .01f), 400, 200), "Alto Puntaje\n" + Managers.Register.TopScore);
        GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .75f), 400, 200), FullText);
		
        string jump = "";

        foreach (int Option in OptionsList.Keys)
        {

            if (Option == ChooseOption)
            {
                GUI.color = Color.white;
                //GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .8f), 400, 200), jump + OptionsList[Option]);
                GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .75f), 400, 200), jump + OptionsList[Option]);
            }

            jump += System.Environment.NewLine;
        }

        GUI.color = Color.white;
    }

 
    public override void Pause()
    {
        //DisplayMenu = false;
    }

    public override void Resume()
    {
        //DisplayMenu = true;

//        Managers.Audio.StopMusic();
//		Managers.Audio.PlayMusic(Opening, .75f, 1);
    }
  
}