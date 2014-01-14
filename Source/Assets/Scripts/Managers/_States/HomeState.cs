using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BonusObjects
{

public class HomeState : GameState 
{
    GUISkin gSkinB                  = null;
//    float Increment                 = 0;
    float prevVolume                = 0;

    uint ChooseOption = 0;
    uint TotalOptions = 0;
    List<string> OptionsList = new List<string>();

    public AudioClip Clang;
    public AudioClip Select;
    public AudioClip Play;
    public AudioClip HealthRefill;
    public AudioClip Doors;

    uint ChooseMusic = 0;
    uint TotalMusics = 0;
    public AudioClip[] MusicList;

    uint ChoosePic = 0;
    uint TotalPics = 0;
    uint PrevPic = 0;
    public Texture2D[] PicsList;

    uint ChooseComic = 0;
    uint TotalComics = 0;
    uint PrevComic = 0;
    public Texture2D[] ComicsList;

    int ChooseWorld = 0;
    int TotalWorlds = 0;
    public string[] WorldsList ;


    public Texture2D Background     = null;
    Rect BackPos = new Rect(0, 0, Screen.width, Screen.height);
    Rect BackCoord = new Rect(0, 0, 1, .75f);

    public Texture2D Box            = null;
    Rect        BoxPos              = new Rect(0, 0, 512, 512);

    public enum HouseState { Start, Menu, Concept, ElseWorlds, Comics };
    public HouseState currentState = HouseState.Start;


    public override void Init()
    {
        //Managers.Game.IsPaused = true;
        //Time.timeScale = 0.00000000000001f;
        Managers.Display.ShowFlash(.5f);
        Managers.Audio.Play(Doors, Managers.Display.camTransform);

        if (Managers.Game.PlayerPrefab)
        {
            Managers.Game.PlayerPrefab.GetComponent<PlayerControls>().enabled = false;
            Managers.Game.IsPaused = true;
        }
            //Managers.Game.PlayerPrefab.SetActive(false); 

        gSkinB = Resources.Load("GUI/GUISkin B") as GUISkin;
        //gSkinB.label.fontSize = Mathf.RoundToInt(Screen.width * 0.035f);
        gSkinB.label.fontSize = 16;

        OptionsList.Add( "Salir");

        if (Managers.Register.Treasure1)
            OptionsList.Add("Tomar Mate");
        if (Managers.Register.Treasure2)
            OptionsList.Add("Escuchar Musica:");
        if (Managers.Register.Treasure3)
            OptionsList.Add("Ver Concept Arts");
        if (Managers.Register.Treasure4)
            OptionsList.Add("Portal Dimensional:");
        if (Managers.Register.Treasure5)
            OptionsList.Add("Leer Historietas");


        TotalOptions = (uint)OptionsList.Count;
        TotalMusics = (uint)MusicList.Length;
        TotalPics = (uint)PicsList.Length;
        TotalComics = (uint)ComicsList.Length;
        TotalWorlds = WorldsList.Length;

        prevVolume = Managers.Audio.Music.volume;
        Managers.Audio.Music.volume = .65f;
        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        BackCoord = new Rect(.05f, .05f, .85f, .55f);

        //BoxPos = new Rect(Screen.width * .5f - 256, (Screen.height * .5f) - (Box.height), 512, 256);
        BoxPos = new Rect(Screen.width * .5f - 164, (Screen.height * .5f) - (Background.height * .45f), 364, 200);


    }

    public override void DeInit()
    {
        //Managers.Game.IsPaused = false;
        //Time.timeScale = 1;
        if (Managers.Game.PlayerPrefab)
        {
            Managers.Game.PlayerPrefab.GetComponent<PlayerControls>().enabled = true;
            Managers.Game.IsPaused = false;
        }
        //if (Managers.Game.PlayerPrefab)
        //    Managers.Game.PlayerPrefab.SetActive(true); 

        TotalOptions = 0;
        OptionsList.Clear();
//        Increment = 0;
        gSkinB.label.fontSize = Mathf.RoundToInt(Screen.width * 0.035f);
        currentState = HouseState.Start;

        Managers.Audio.Music.volume = prevVolume;

    }
	

    public override void OnUpdate()
    {
        if (Input.GetKeyDown("escape") || Input.GetButtonDown("Start") || Input.GetButtonDown("Select"))
        {

            if (currentState == HouseState.Menu)
            {
                Managers.Audio.Play(Doors, Managers.Display.camTransform);
                Managers.Game.PopState();
            }
            else
            {
                Managers.Audio.Play(Clang, Managers.Display.camTransform);
                currentState = HouseState.Menu;
            }
        }


        switch (currentState)
        {
            case HouseState.Start:
                currentState = HouseState.Menu;
                break;

            case HouseState.Menu:

                if (Managers.Game.InputDown && (ChooseOption > 0))
                {
                    Managers.Audio.Play(Select, Managers.Display.camTransform);
                    ChooseOption--;
                }
                if (Managers.Game.InputUp && (ChooseOption < (TotalOptions - 1)))
                {
                    Managers.Audio.Play(Select, Managers.Display.camTransform);
                    ChooseOption++;
                }

                    switch (OptionsList[(int)ChooseOption])
                    {
                        case "Salir":             // Salir
                            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
                            {
                                Managers.Audio.Play(Doors, Managers.Display.camTransform);

                                Managers.Display.ShowFlash(.5f);
                                Managers.Game.PopState();
                            }
                            break;

                        case "Tomar Mate":             // Mate
                            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
                            {
                                Managers.Display.ShowFlash(.25f);
                                //Managers.Display.ShowStatus();
                                Managers.Audio.Play(HealthRefill, Managers.Display.camTransform );
                                Managers.Register.Health = 3;
                            }
                            break;

                        case "Ver Concept Arts":             // Arte Concept
                            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
                            {
                                Managers.Audio.Play(Play, Managers.Display.camTransform);
                                Managers.Display.ShowFlash(.2f);
                                currentState = HouseState.Concept;
                                Managers.Game.IsPlaying = false;
                            }
                            break;

                        case "Escuchar Musica:":             // Musica

                            if (Managers.Game.InputLeft && ChooseMusic > 0)
                            {
                                Managers.Audio.Play(Select, Managers.Display.camTransform);
                                ChooseMusic--;
                            }
                            if (Managers.Game.InputRight && ChooseMusic < TotalMusics - 1)
                            {
                                Managers.Audio.Play(Select, Managers.Display.camTransform);
                                ChooseMusic++;
                            }

                            //ChooseMusic = (uint)Mathf.Clamp(ChooseMusic, 0, TotalMusics-1);

                            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
                            {

                                if ((Managers.Audio.Music.clip == MusicList[ChooseMusic]) && Managers.Audio.Music.isPlaying)
                                {
                                    Managers.Audio.Play(Clang, Managers.Display.camTransform);
                                    Managers.Audio.StopMusic();
                                }
                                else
                                {
                                    Managers.Audio.PlayMusic(MusicList[ChooseMusic], 0.65f, 1);
                                    Managers.Audio.Play(Play, Managers.Display.camTransform);
                                }
                            }
                            break;

                        case "Portal Dimensional:":             // Inter-Dimensional Devicet

                            if (Managers.Game.InputLeft && ChooseWorld > 0)
                            {
                                Managers.Audio.Play(Select, Managers.Display.camTransform);
                                ChooseWorld--;
                            }
                            if (Managers.Game.InputRight && ChooseWorld < TotalWorlds - 1)
                            {
                                Managers.Audio.Play(Select, Managers.Display.camTransform);
                                ChooseWorld++;
                            }

                            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
                            {
                                Managers.Audio.Play(Play, Managers.Display.camTransform);
                                Managers.Display.ShowFlash(1.5f);

                                if (WorldsList[ChooseWorld] == "Sarasa")
                                {
                                    Managers.Display.ShowFlash(1.5f);
                                    LoadWorld("/Levels/ElseWorld1.tmx");
                                }
                                else if (WorldsList[ChooseWorld] == "Jarana")
                                {
                                    Managers.Display.ShowFlash(1.5f);
                                    LoadWorld("/Levels/ElseWorld2.tmx");
                                }
                                else
                                {
                                    Managers.Display.ShowFlash(1.5f);
                                    LoadWorld("/Levels/ElseWorld3.tmx");
                                }
                             
                            }
                            break;
                        case "Leer Historietas":             // Comic
                            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
                            {
                                Managers.Audio.Play(Play, Managers.Display.camTransform);
                                Managers.Display.ShowFlash(.2f);
                                currentState = HouseState.Comics;
                                Managers.Game.IsPlaying = false;
                            }
                            break;
                    }
      
                break;

            case HouseState.Concept:

                if (Managers.Game.InputLeft && ChoosePic > 0)
                {
                    Managers.Audio.Play(Clang, Managers.Display.camTransform);
                    ChoosePic--;
                }
                if (Managers.Game.InputRight && ChoosePic < TotalPics - 1)
                {
                    Managers.Audio.Play(Clang, Managers.Display.camTransform);
                    ChoosePic++;
                }

                if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetKeyDown("escape") ||
                 Input.GetKeyDown("escape") || Input.GetButtonDown("Fire3") || Input.GetButtonDown("Select"))
                {
                    Managers.Audio.Play(Play, Managers.Display.camTransform);
                    Managers.Display.ShowFlash(.5f);
                    currentState = HouseState.Menu;
                    Managers.Game.IsPlaying = true;

                }
                break;

            case HouseState.ElseWorlds:
                break;

            case HouseState.Comics:
                if (Managers.Game.InputLeft && ChooseComic > 0)
                {
                    Managers.Audio.Play(Clang, Managers.Display.camTransform);
                    ChooseComic--;
                }

                if (Managers.Game.InputRight && ChooseComic < TotalComics - 1)
                {
                    Managers.Audio.Play(Clang, Managers.Display.camTransform);
                    ChooseComic++;
                }

                if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetKeyDown("escape") ||
                 Input.GetKeyDown("escape") || Input.GetButtonDown("Fire3") || Input.GetButtonDown("Select"))
                {
                    Managers.Audio.Play(Play, Managers.Display.camTransform);
                    Managers.Display.ShowFlash(.5f);
                    currentState = HouseState.Menu;
                    Managers.Game.IsPlaying = true;

                }
                break;
        }

    }

    void LoadWorld(string world)
    {
        Managers.Display.ShowFlash(1);
        Managers.Tiled.Unload();

        if (Managers.Tiled.Load(world))
        {
            Managers.Game.PushState(typeof(DummyState));
            Input.ResetInputAxes();
            return;
        }
    }

    public override void OnRender()
    {
        if (gSkinB) GUI.skin = gSkinB;

        GUI.color = Color.white;
        GUI.depth = 1;
        GUI.DrawTextureWithTexCoords(BackPos, Background, BackCoord);

        if ( Managers.Register.Treasure4 )
        GUI.DrawTextureWithTexCoords(new Rect(Screen.width * .2f, Screen.height * .8f, 128, 128),
            Background, new Rect(0, .875f, .125f, .125f)); // VG device

        if (Managers.Register.Treasure2)
        GUI.DrawTextureWithTexCoords(new Rect(Screen.width * .2f, Screen.height * .6f, 128, 128),
            Background, new Rect(.125f, .875f, .125f, .125f)); // Radio

        if (Managers.Register.Treasure3)
        GUI.DrawTextureWithTexCoords(new Rect(Screen.width * .55f, Screen.height * .475f, 128, 128),
            Background, new Rect(.25f, .875f, .125f, .125f)); // Art

        if (Managers.Register.Treasure5)
        GUI.DrawTextureWithTexCoords(new Rect(Screen.width * .7f, Screen.height * .8f, 128, 128),
            Background, new Rect(.375f, .875f, .125f, .125f)); // Comics

        if (Managers.Register.Treasure1)
        GUI.DrawTextureWithTexCoords(new Rect(Screen.width * .61f, Screen.height * .665f, 128, 128),
            Background, new Rect(.5f, .875f, .125f, .125f)); // Mate


        GUI.color = new Color(1, 0.36f, 0.22f, 1);


        switch (currentState)
        {
            case HouseState.Start:
                Managers.Display.ShowFlash(1);
                break;

            case HouseState.Menu:

                //GUI.DrawTexture(BoxPos, Box);
                GUI.DrawTextureWithTexCoords(BoxPos, Background, new Rect(.625f, .875f, .375f, .125f));

                if (OptionsList.Count < 2)
                    GUI.Label(new Rect((Screen.width * .5f) - 150,
                                     (Screen.height * .5f) - (Background.height * .15f) - 150, 400, 200),
                                     "Esta pocilga dá pena, está\nalgo vacía y abandonada. \nBuscá Tesoros para mejorar\ntu hogar y Date una vuelta\nde véz en cuando para\nno perderte de alguna\nque otra sorpresa, Suerte!");


                for (int Option = 0; Option < OptionsList.Count; Option++ )
                {
                    GUI.color = new Color(1, 0.36f, 0.22f, 1);


                    if (Option == ChooseOption)
                        GUI.color = Color.white;

                    GUI.Label(new Rect((Screen.width * .5f) - 150,
                        (Screen.height * .5f) - (Background.height * .15f) - 24 * Option, 400, 200), OptionsList[Option]);

                    if (OptionsList[Option] == "Escuchar Musica:")  // Audio
                        GUI.Label(new Rect((Screen.width * .5f) + 84,
                         (Screen.height * .5f) - (Background.height * .15f) - 24 * Option, 400, 200), MusicList[ChooseMusic].name);

                    if (OptionsList[Option] == "Portal Dimensional:")  // Levels
                        GUI.Label(new Rect((Screen.width * .5f) + 84,
                         (Screen.height * .5f) - (Background.height * .15f) - 24 * Option, 400, 200), WorldsList[ChooseWorld]);
                }

             

                //if (Managers.Register.Treasure3)  // Audio
                //    GUI.Label(new Rect((Screen.width * .5f) + 64,
                //     (Screen.height * .5f) + (Box.height * .45f) - 96, 400, 200), MusicList[ChooseMusic].name);


                GUI.color = Color.white;
                break;

            case HouseState.Comics:
                GUI.color = Color.white;

                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), ComicsList[ChooseComic]);

                if (PrevComic != ChooseComic)
                {
                    Managers.Display.ShowFlash(.1f);
                    PrevComic = ChooseComic;
                }
                break;

            case HouseState.ElseWorlds:
                break;

            case HouseState.Concept:

                //GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), Box);
                GUI.color = Color.white;
                
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), PicsList[ChoosePic]);

                if (PrevPic != ChoosePic)
                {
                    Managers.Display.ShowFlash(.1f);
                    PrevPic = ChoosePic;
                }

                break;
        }

        //if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return"))
        //    Managers.Display.ShowFlash(.5f);
    }

    public override void Pause() 
    {
        Managers.Display.ShowFlash(1);
        Managers.Audio.Music.volume = .45f;

        if (Managers.Game.PlayerPrefab)
        {
            Managers.Game.PlayerPrefab.GetComponent<PlayerControls>().enabled = true;
            Managers.Game.IsPaused = false;
        }

    }
    public override void Resume() 
    {
        Managers.Display.ShowFlash(1);
        gSkinB.label.fontSize = 16;
        Managers.Audio.Music.volume = .65f;


        if (Managers.Game.PlayerPrefab)
        {
            Managers.Game.IsPlaying = true;

            if (Managers.Register.YaguaDefeated && Managers.Register.MonaiDefeated && !Managers.Register.YasiYatereDefeated)
            {
                if (Managers.Register.UnlockedStages <= 3)
                    Managers.Register.UnlockedStages = 4;
                Managers.Tiled.Load("/Levels/Home2.tmx");
            }
            else if (Managers.Register.YasiYatereDefeated)
            {
                if (Managers.Register.UnlockedStages <= 4)
                    Managers.Register.UnlockedStages = 5;
                Managers.Tiled.Load("/Levels/Home3.tmx");
            }
            else
                Managers.Tiled.Load("/Levels/Home1.tmx");

            //Managers.Display.camTransform.position = Vector3.zero;
            Managers.Display.camTransform.position = new Vector3(3.75f, 2.5f, 0);


            //Managers.Game.IsPlaying = false;
            Managers.Game.PlayerPrefab.GetComponent<PlayerControls>().enabled = false;
            Managers.Game.IsPaused = true;
 
        }

        currentState = HouseState.Menu;

        //Managers.Game.IsPaused = true;
        //Time.timeScale = 0.00000000000001f;    
    }


}



}