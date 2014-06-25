
using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : GameState
{
	public bool MusicIntro = true;
	public float timeTrailer = 30f;
	private Dictionary<int, string> OptionsList = new Dictionary<int, string>();
	private string FullText = string.Empty;
	public bool isLoading;
	public bool DisplayMenu;
	public bool DisplayInfo;
	private uint ChooseOption;
	private uint TotalOptions;
	public AudioClip PreOpening;
	public AudioClip Opening;
	private GameObject IntroPrefab;
	private GUISkin gSkin;
	public Texture2D Base;
	private Camera camBack;
	private Camera camMiddle;
	private Camera camFront;
	private Camera MainCam;
	private Transform SkyTransform;
	private Transform CamTransform;
	
	public override void Init()
	{
		this.timeTrailer = 30f;
		Managers.Audio.StopMusic();
		Managers.Audio.Play(this.PreOpening, Managers.Display.MainCamera.transform, 1f, 1f);
		this.gSkin = (GUISkin) Resources.Load("GUI/GUISkin B", typeof (GUISkin));
		this.IntroPrefab = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Intro/IntroPrefab", typeof (GameObject)));
		Managers.Display.MainCamera.enabled = false;
		Managers.Display.MainCamera.tag = "Untagged";
		this.camBack = GameObject.Find("Camera_Back").GetComponent<Camera>();
		this.camMiddle = GameObject.Find("Camera_Middle").GetComponent<Camera>();
		this.camFront = GameObject.Find("Camera_Front").GetComponent<Camera>();
		this.MainCam = GameObject.Find("Camera_Ortho").GetComponent<Camera>();
		this.MainCam.tag = "MainCamera";
		this.CamTransform = this.MainCam.transform;
		this.SkyTransform = GameObject.Find("Intro_Clouds").transform;
		if ((bool) ((UnityEngine.Object) this.camFront))
		{
			this.MainCam.orthographicSize = 0.01f;
			this.camFront.fov = 170f;
			this.camMiddle.fov = 170f;
			this.camBack.fov = 170f;
		}
		if (Managers.Register.PlayTutorialFirst)
		{
			this.OptionsList.Add(0, "Comenzar");
			this.OptionsList.Add(1, "Opciones");
			this.OptionsList.Add(2, "Salir");
		}
		else
		{
			this.OptionsList.Add(0, "Continuar");
			this.OptionsList.Add(1, "Tutorial");
			this.OptionsList.Add(2, "Opciones");
			this.OptionsList.Add(3, "Salir");
		}
		this.TotalOptions = (uint) this.OptionsList.Count;
		using (Dictionary<int, string>.KeyCollection.Enumerator enumerator = this.OptionsList.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				MainMenuState mainMenuState = this;
				string str = mainMenuState.FullText + this.OptionsList[current] + Environment.NewLine;
				mainMenuState.FullText = str;
			}
		}
	}
	
	public override void DeInit()
	{
		Managers.Display.MainCamera.enabled = true;
		Managers.Display.MainCamera.tag = "MainCamera";
		UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.IntroPrefab);
		this.IntroPrefab = (GameObject) null;
		this.camBack = (Camera) null;
		this.camMiddle = (Camera) null;
		this.camFront = (Camera) null;
		this.MainCam = (Camera) null;
		this.CamTransform = (Transform) null;
		this.OptionsList.Clear();
		this.FullText = string.Empty;
		Managers.Audio.StopMusic();
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

		this.CamTransform.position = new Vector3((float) Math.Round((double) Mathf.Sin(Time.time) * 0.0500000007450581, 4), this.CamTransform.position.y, this.CamTransform.position.z);
		if ((bool) ((UnityEngine.Object) this.SkyTransform))
			this.SkyTransform.Rotate(Vector3.forward * Time.deltaTime * 10f);
		this.DisplayMenu = (double) this.camFront.fov <= 58.0;
		if ((double) this.MainCam.orthographicSize < 0.899999976158142)
			this.MainCam.orthographicSize += Time.deltaTime;
		if (!this.DisplayMenu)
		{
			float num = 40f;
			if (Input.anyKey)
				num = 100f;
			this.camBack.fov -= Time.deltaTime * num;
			this.camMiddle.fov -= Time.deltaTime * num;
			this.camFront.fov -= Time.deltaTime * num;
		}
		else
		{
			if (Managers.Game.InputUp && this.ChooseOption > 0U && !this.DisplayInfo)
				--this.ChooseOption;
			if (Managers.Game.InputDown && this.ChooseOption < this.TotalOptions - 1U && !this.DisplayInfo)
				++this.ChooseOption;
			if (!Input.GetButtonDown("Fire1") && !Input.GetKeyDown("return") && !Input.GetButtonDown("Start"))
				return;
			if (this.TotalOptions < 4U)
			{
				switch (this.ChooseOption)
				{
				case 0U:
					Managers.Display.ShowFlash(1f);
					Managers.Game.ChangeState(typeof (MapState));
					break;
				case 1U:
					Managers.Display.ShowFlash(0.25f);
					Managers.Game.PushState(typeof (ConfigMenuState));
					break;
				case 2U:
					Managers.Display.ShowFlash(1f);
					Application.Quit();
					break;
				}
			}
			else
			{
				switch (this.ChooseOption)
				{
				case 0U:
					Managers.Display.ShowFlash(1f);
					Managers.Game.ChangeState(typeof (MapState));
					break;
				case 1U:
					Managers.Display.ShowFlash(1f);
					Managers.Register.FirstTimePlay = true;
					Managers.Register.PlayTutorialFirst = true;
					Managers.Game.ChangeState(typeof (MapState));
					break;
				case 2U:
					Managers.Display.ShowFlash(0.25f);
					Managers.Game.PushState(typeof (ConfigMenuState));
					break;
				case 3U:
					Managers.Display.ShowFlash(1f);
					Application.Quit();
					break;
				}
			}
		}
	}
	
	public override void OnRender()
	{
		if (!this.DisplayMenu)
			return;
		if ((bool) ((UnityEngine.Object) this.Opening) && !Managers.Audio.Music.isPlaying && (Managers.Audio.SoundEnable && this.MusicIntro))
			Managers.Audio.PlayMusic(this.Opening, 1f, 1f);
		if ((bool) ((UnityEngine.Object) this.gSkin))
		{
			GUI.skin = this.gSkin;
			GUI.skin.label.fontSize = 32;
			GUI.color = new Color(1f, 0.36f, 0.22f, 1f);
		}
		else
			Debug.Log((object) "MainMenuGUI : GUI skin object missing!");
		if (this.isLoading)
		{
			GUI.Label(new Rect((float) ((double) Screen.width * 0.5 - 100.0), (float) Screen.height * 0.5f, 400f, 50f), "Loading...");
		}
		else
		{
        	GUI.Label(new Rect((Screen.width * .01f), (Screen.height * .875f), 400, 200), "DEMO\nVERSION"  );
			
			GUI.DrawTexture(new Rect((float) Screen.width * 0.625f, (float) Screen.height * 0.725f, 400f, 200f), (Texture) this.Base);
			GUI.color = new Color(1f, 0.36f, 0.22f, 1f);
			GUI.Label(new Rect((float) Screen.width * 0.65f, (float) Screen.height * 0.75f, 400f, 200f), this.FullText);
			string str = string.Empty;
			using (Dictionary<int, string>.KeyCollection.Enumerator enumerator = this.OptionsList.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int current = enumerator.Current;
					if ((long) current == (long) this.ChooseOption)
					{
						GUI.color = Color.white;
						GUI.Label(new Rect((float) Screen.width * 0.65f, (float) Screen.height * 0.75f, 400f, 200f), str + this.OptionsList[current]);
					}
					str = str + Environment.NewLine;
				}
			}
			GUI.color = Color.white;
		}
	}
	
	public override void Pause()
	{
		this.OptionsList.Clear();
		this.FullText = string.Empty;
	}
	
	public override void Resume()
	{
		if (Managers.Register.PlayTutorialFirst)
		{
			this.OptionsList.Add(0, "Comenzar");
			this.OptionsList.Add(1, "Opciones");
			this.OptionsList.Add(2, "Salir");
		}
		else
		{
			this.OptionsList.Add(0, "Continuar");
			this.OptionsList.Add(1, "Tutorial");
			this.OptionsList.Add(2, "Opciones");
			this.OptionsList.Add(3, "Salir");
		}
		this.TotalOptions = (uint) this.OptionsList.Count;
		using (Dictionary<int, string>.KeyCollection.Enumerator enumerator = this.OptionsList.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				MainMenuState mainMenuState = this;
				string str = mainMenuState.FullText + this.OptionsList[current] + Environment.NewLine;
				mainMenuState.FullText = str;
			}
		}
	}
}


//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class MainMenuState : GameState 
//{
//    public bool isLoading = false;
//    public bool DisplayMenu = false;
//    public bool DisplayInfo = false;
//    public bool MusicIntro = true;
//
//    public float timeTrailer = 30;
//
//    uint ChooseOption = 0;
//    uint TotalOptions = 0;
//    Dictionary<int, string>  OptionsList = new Dictionary<int, string>();
//
//    string FullText = "";
//
//    public AudioClip PreOpening;
//    public AudioClip Opening;
//    GameObject IntroPrefab;
//    GUISkin gSkin;
//
//    Camera camBack;
//    Camera camMiddle;
//    Camera camFront;
//    Camera MainCam;
//
//    Transform SkyTransform;
//    Transform CamTransform;
//
//    public override void Init()
//    {
//        timeTrailer = 30;
//
//        Managers.Audio.Play(PreOpening, Managers.Display.MainCamera.transform, 1, 1);
//
//        gSkin = (GUISkin)Resources.Load("GUI/GUISkin B", typeof(GUISkin));
//        IntroPrefab = (GameObject)Instantiate(Resources.Load("Prefabs/Intro/IntroPrefab", typeof(GameObject)) );
//
//        Managers.Display.MainCamera.enabled = false;
//        Managers.Display.MainCamera.tag = "Untagged";
//
//        camBack = GameObject.Find("Camera_Back").GetComponent<Camera>() as Camera;
//        camMiddle = GameObject.Find("Camera_Middle").GetComponent<Camera>() as Camera;
//        camFront = GameObject.Find("Camera_Front").GetComponent<Camera>() as Camera;
//
//        MainCam = GameObject.Find("Camera_Ortho").GetComponent<Camera>() as Camera;
//        MainCam.tag = "MainCamera";
//        CamTransform = MainCam.transform;
//        SkyTransform = GameObject.Find("Intro_Clouds").transform;
//
//
//        if (camFront)
//        {
//            MainCam.orthographicSize = 0.01f;
//            camFront.fieldOfView = 170;
//			camMiddle.fieldOfView = 170;
//			camBack.fieldOfView = 170;
//        }
//
//        OptionsList.Add(0, "Comenzar");
//        OptionsList.Add(1, "Opciones");
//        OptionsList.Add(2, "Salir");
//        //OptionsList.Add(0, "Comenzar");
//        //OptionsList.Add(1, "Records");
//        //OptionsList.Add(2, "Opciones");
//        //OptionsList.Add(3, "Salir");
//        TotalOptions = (uint)OptionsList.Count;
//
//        foreach (int Option in OptionsList.Keys)
//            FullText += (OptionsList[Option] + System.Environment.NewLine); // This it's a fix to reduce the options drawcalls
//
//        //Managers.Game.TopScore = PlayerPrefs.GetInt("TopScore");
//    }
//	
//    public override void DeInit()
//    {
//        Managers.Display.MainCamera.enabled = true;
//        Managers.Display.MainCamera.tag = "MainCamera";
//
//        DestroyImmediate(IntroPrefab);
//        IntroPrefab = null;
//        camBack = null;
//        camMiddle = null;
//        camFront = null;
//        MainCam = null;
//        CamTransform = null;
//        OptionsList.Clear();
//
//        FullText = "";
//        Managers.Audio.StopMusic();
//
//        //PlayerPrefs.SetInt("TopScore", (int)Managers.Game.TopScore);
//
//    }
//
//    public override void OnUpdate()
//    {
//  
////		if (Input.anyKeyDown)
////        	timeTrailer = 10;
////
////        timeTrailer -= Time.deltaTime;
////
////        if (timeTrailer <= 0)
////        {
////            Managers.Game.ChangeState(typeof(TrailerState));
////            return;
////        }
//
//        CamTransform.position = new Vector3((float)System.Math.Round((Mathf.Sin(Time.time) * 0.05f), 4),
//                                            CamTransform.position.y, CamTransform.position.z); // OJo aca
//
//        if (SkyTransform)
//            SkyTransform.Rotate(Vector3.forward * Time.deltaTime * 10);
//
//		DisplayMenu = (camFront.fieldOfView <= 58);
//
//
//        if (MainCam.orthographicSize < 0.9f)
//            MainCam.orthographicSize += Time.deltaTime;
//
//        if (!DisplayMenu)
//        {
//            float speed = 40;
//            if (Input.anyKey)
//                speed = 100;
//
//			camBack.fieldOfView -= Time.deltaTime * speed;
//			camMiddle.fieldOfView -= Time.deltaTime * speed;
//			camFront.fieldOfView -= Time.deltaTime * speed;
//            return;
//        }
//
//        ///////////////////////////////////////////////////////////////////////////////////////
//
//        //if (!DisplayMenu)
//        //    return;
//
//        if (Managers.Game.InputUp && (ChooseOption > 0) && !DisplayInfo)
//            ChooseOption--;
//        if (Managers.Game.InputDown && (ChooseOption < (TotalOptions - 1)) && !DisplayInfo)
//            ChooseOption++;
//
//        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetButtonDown("Start"))
//            switch (ChooseOption)
//            {
//                case 0:
//                    Managers.Display.ShowFlash(1);
//                    Managers.Game.ChangeState(typeof(MapState));
//                    break;
//                case 1:
//                    Managers.Display.ShowFlash(.25f);
//                    Managers.Game.PushState(typeof(ConfigMenuState));
//                    break;
//                case 2:
//                    Managers.Display.ShowFlash(1);
//                    Application.Quit();
//                    break;
//                //case 0:
//                //    Managers.Display.ShowFlash(1);
//                //    Managers.Game.ChangeState(typeof(MapState));
//                //    break;
//                //case 1:
//                //    Managers.Display.ShowFlash(.25f);
//                //    Managers.Game.PushState(typeof(TopScores));
//                //    break;
//                //case 2:
//                //    Managers.Display.ShowFlash(.25f);
//                //    Managers.Game.PushState(typeof(ConfigMenuState));
//                //    break;
//                //case 3:
//                //    Managers.Display.ShowFlash(1);
//                //    Application.Quit();
//                //    break;
//            }
//    }
//
//    public override void OnRender()
//    {
//        if (!DisplayMenu)
//            return;
//
//        if (Opening && !Managers.Audio.Music.isPlaying && Managers.Audio.SoundEnable && MusicIntro)
//            Managers.Audio.PlayMusic(Opening, 1, 1);
//
//        if (gSkin)
//        {
//            GUI.skin = gSkin;
//            GUI.skin.label.fontSize = 32;
//            GUI.color = new Color(1, 0.36f, 0.22f, 1);
//        }
//        else Debug.Log("MainMenuGUI : GUI skin object missing!");
//		
//        GUI.Label(new Rect((Screen.width * .01f), (Screen.height * .875f), 400, 200), "DEMO\nVERSION"  );
//		
//        if (isLoading)
//        {
//            GUI.Label(new Rect((Screen.width * .5f) - 100, (Screen.height * .5f), 400, 50), "Loading...");
//            return;
//        }
//
//
//        GUI.color = new Color(1, 0.36f, 0.22f, 1);
//        GUI.Label(new Rect((Screen.width * .01f), (Screen.height * .01f), 400, 200), "Alto Puntaje\n" + Managers.Register.TopScore);
//        GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .75f), 400, 200), FullText);
//		
//        string jump = "";
//
//        foreach (int Option in OptionsList.Keys)
//        {
//
//            if (Option == ChooseOption)
//            {
//                GUI.color = Color.white;
//                //GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .8f), 400, 200), jump + OptionsList[Option]);
//                GUI.Label(new Rect((Screen.width * .65f), (Screen.height * .75f), 400, 200), jump + OptionsList[Option]);
//            }
//
//            jump += System.Environment.NewLine;
//        }
//
//        GUI.color = Color.white;
//    }
//
// 
//    public override void Pause()
//    {
//        //DisplayMenu = false;
//    }
//
//    public override void Resume()
//    {
//        //DisplayMenu = true;
//
////        Managers.Audio.StopMusic();
////		Managers.Audio.PlayMusic(Opening, .75f, 1);
//    }
//  
//}