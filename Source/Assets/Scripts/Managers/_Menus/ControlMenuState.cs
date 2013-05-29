using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlMenuState : GameState 
{
    public Texture AutoRunInfo;
    public Texture ClassicRunInfo;

    uint ChooseOption = 0;
    uint TotalOptions = 0;
    Dictionary<int, string> OptionsList = new Dictionary<int, string>();

    GameObject IntroPrefab;
    GUISkin gSkin;



    public override void Init()
    {
        gSkin = (GUISkin)Resources.Load("GUI/GUISkin B", typeof(GUISkin));


        OptionsList.Add(0, "Modo Correr:");
        OptionsList.Add(1, "Volver");
        TotalOptions = (uint)OptionsList.Count;

    }

    public override void DeInit()
    {
        TotalOptions = 0;
        OptionsList.Clear();
    }
	
    public override void OnUpdate()
    {
        if (  Managers.Game.InputUp  && (ChooseOption > 0))
            ChooseOption--;
        if ( Managers.Game.InputDown  && (ChooseOption < (TotalOptions - 1)))
            ChooseOption++;

        if (Input.GetKeyDown("escape"))
        {
            Managers.Game.PopState();
            return;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetButtonDown("Start"))
            switch (ChooseOption)
            {
                case 0:
                    Managers.Register.PlayerAutoRunning = !Managers.Register.PlayerAutoRunning;
                    break;
                case 1:
                    Managers.Game.PopState();
                    break;
            }
    }

    public override void OnRender()
    {

        if (gSkin)
        {
            GUI.skin = gSkin;
            GUI.color = new Color(1, 0.36f, 0.22f, 1);

            GUI.Box(new Rect((Screen.width * .5f) - (Screen.width * .3f), (Screen.height * .5f) - (Screen.height * .35f),
                                                        (Screen.width * .6f), (Screen.height * .7f)), ""  );

            if (Managers.Register.PlayerAutoRunning)
                GUI.DrawTexture(new Rect( (Screen.width * .5f) - (AutoRunInfo.width * .45f), (Screen.height * .5f) - (Screen.height * .35f),
                                        ClassicRunInfo.width * .9f, Screen.height * .75f), AutoRunInfo);
            else
                GUI.DrawTexture(new Rect((Screen.width * .5f) - (AutoRunInfo.width * .45f), (Screen.height * .5f) - (Screen.height * .35f),
                                         ClassicRunInfo.width * .9f, Screen.height * .75f), ClassicRunInfo);

            GUI.skin.label.fontSize = 32;
            GUI.color = Color.white;
        }


        GUI.color = new Color(1, 0.36f, 0.22f, 1);
        string jump = "";

        foreach (int Option in OptionsList.Keys)
        {
            if (Option == 1)
                jump = "\n\n\n\n\n\n\n\n\n"; 

            if (Option == ChooseOption)
                GUI.color = Color.white;
            else
                GUI.color = new Color(1, 0.36f, 0.22f, 1);


            GUI.Label(new Rect((Screen.width * .5f) - (Screen.width * .25f), (Screen.height * .2f), 400, 200), jump + OptionsList[Option]);
        }

        GUI.color = Color.white;

        if (Managers.Register.PlayerAutoRunning )
            GUI.Label(new Rect((Screen.width * .5f) - 50, (Screen.height * .2f), 400, 200), "\n Automatico");
        else
            GUI.Label(new Rect((Screen.width * .5f) - 50, (Screen.height * .2f), 400, 200), "\n Clasico");


    }

    public override void Pause() { ;}
    public override void Resume() { ;}

}