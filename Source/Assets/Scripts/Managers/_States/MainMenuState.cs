using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuState : GameState 
{
    public bool isLoading = false;
    public bool DisplayMenu = false;
    public bool DisplayInfo = false;

    uint ChooseOption = 0;
    uint TotalOptions = 0;
    Dictionary<int, string>  OptionsList = new Dictionary<int, string>();

    //public string StartLevel = "JumpGameSinglePlayer";
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

        //Managers.Audio.Play(PreOpening, CamTransform, 1, 1);

        if (camFront)
        {
            MainCam.orthographicSize = 0.01f;
            camFront.fov = 170;
            camMiddle.fov = 170;
            camBack.fov = 170;
        }

        OptionsList.Add(0, "Start Game");
        OptionsList.Add(1, "Help/Info");
        OptionsList.Add(2, "Quit");
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
        CamTransform.position = new Vector3((float)System.Math.Round((Mathf.Sin(Time.time) * 0.05f), 4),
                                            CamTransform.position.y, CamTransform.position.z); // OJo aca

        if (SkyTransform)
            SkyTransform.Rotate(Vector3.forward * Time.deltaTime * 10);

        DisplayMenu = (camFront.fov <= 58);


        if (MainCam.orthographicSize < 0.9f)
            MainCam.orthographicSize += Time.deltaTime;

        if (!DisplayMenu)
        {
            float speed = 40;
            if (Input.anyKey)
                speed = 100;

            camBack.fov -= Time.deltaTime * speed;
            camMiddle.fov -= Time.deltaTime * speed;
            camFront.fov -= Time.deltaTime * speed;
            return;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        //if (!DisplayMenu)
        //    return;

        if (Input.GetKeyDown("up") && (ChooseOption > 0) && !DisplayInfo)
            ChooseOption--;
        if (Input.GetKeyDown("down") && (ChooseOption < (TotalOptions - 1)) && !DisplayInfo)
            ChooseOption++;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
            switch (ChooseOption)
            {
                case 0:
                    Managers.Display.ShowFlash(1);
                    Managers.Game.ChangeState(typeof(MapState));
                    break;
                case 1:
                    Managers.Display.ShowFlash(.25f);
                    DisplayInfo = !DisplayInfo;
                    break;
                case 2:
                    Managers.Display.ShowFlash(1);
                    Application.Quit();
                    break;
            }
    }

    public override void OnRender()
    {
        if (!DisplayMenu)
            return;

        if (Opening && !Managers.Audio.Music.isPlaying && Managers.Audio.SoundEnable)
            Managers.Audio.PlayMusic(Opening, 1, 1);

        if (gSkin)
        {
            GUI.skin = gSkin;
            GUI.skin.label.fontSize = 32;
            GUI.color = new Color(1, 0.36f, 0.22f, 1);
        }
        else Debug.Log("MainMenuGUI : GUI skin object missing!");

        if (isLoading)
        {
            GUI.Label(new Rect((Screen.width * .5f) - 100, (Screen.height * .5f), 400, 50), "Loading...");
            return;
        }

        if (DisplayInfo)
        {
            GUI.color = new Color(1, 0.36f, 0.22f, 1);
            GUI.Box(new Rect((Screen.width * .5f) - (Screen.width * .3f),
                         (Screen.height * .5f) - (Screen.height * .35f),
                          (Screen.width * .6f), (Screen.height * .7f)),
                "\n CONTROLES: \n Desplazamiento: <- Cursores direccionales -> \n Saltar/Planear: Barra Espacio \n Correr/Agarrar/Tirar: Tecla Control \n Usar Item/Poder Especial: Tecla ALT"
                + "\n\n\n CONTROLS: \n Move: <- Direction Keys -> \n Jump: SpaceBar \n Run & Grab: Control \n PowerUp/UseItem: Alt"
                + "\n\n\n\n Programed By CarliXYZ \n Art & Design By FAC! \n Music By Menta-Beat \n\n ÑANGAPIRY® - 2012 - SUNHOUSE - ");
            //GUI.color = Color.clear;
            GUI.color = Color.white;

            return;
        }

        //GUI.Label(new Rect((Screen.width * .05f) - 40, (Screen.height * .05f) - 10, 400, 100), "Top Score\n" + GameManager.Get().TopScore);

        GUI.color = new Color(1, 0.36f, 0.22f, 1);
        GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .8f), 400, 200), FullText);
        string jump = "";

        foreach (int Option in OptionsList.Keys)
        {

            if (Option == ChooseOption)
            {
                GUI.color = Color.white;
                GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .8f), 400, 200), jump + OptionsList[Option]);
            }

            jump += System.Environment.NewLine;
        }

        GUI.color = Color.white;

        //foreach (int Option in OptionsList.Keys)
        //{

        //    if (Option == ChooseOption)
        //        GUI.color = Color.white;
        //    else
        //        GUI.color = new Color(1, 0.36f, 0.22f, 1);

        //    GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .8f) + Option * 32, 400, 50), OptionsList[Option]);
        //}

        //GUI.color = Color.white;

    }

 
    public override void Pause()
    {
    }

    public override void Resume()
    {
//        Managers.Audio.StopMusic();

    }
  
}