using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigMenuState : GameState 
{

    uint ChooseOption = 0;
    uint TotalOptions = 0;
    Dictionary<int, string> OptionsList = new Dictionary<int, string>();

    string FullText = "\n";

    GUISkin gSkin;


    public override void Init()
    {
        gSkin = (GUISkin)Resources.Load("GUI/GUISkin B", typeof(GUISkin));

        
        OptionsList.Add(0, "Controles");
        OptionsList.Add(1, "Sonido");
        OptionsList.Add(2, "Creditos");
        OptionsList.Add(3, "Volver");
        TotalOptions = (uint)OptionsList.Count;

        foreach (int Option in OptionsList.Keys)
            FullText += (OptionsList[Option] + System.Environment.NewLine); // This it's a fix to reduce the options drawcalls
    }
	

    public override void DeInit()
    {
        TotalOptions = 0;
        OptionsList.Clear();
        FullText = "\n";
    }
	
    public override void OnUpdate()
    {
        if ( Managers.Game.InputUp  && (ChooseOption > 0))
            ChooseOption--;
        if (( Managers.Game.InputDown) && (ChooseOption < (TotalOptions - 1)))
            ChooseOption++;

        if (Input.GetKeyDown("escape") || Input.GetButtonDown("Select"))
        {
            Managers.Game.PopState();
            return;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetButtonDown("Start"))
            switch (ChooseOption)
            {
                case 0:
                    Managers.Game.PushState(typeof(ControlMenuState));
                    break;
                case 1:
                    Managers.Game.PushState(typeof(SoundMenuState));
                    break;
                case 2:
                    Managers.Game.PushState(typeof(CreditsMenuState));
                    break;
                case 3:
                    Managers.Game.PopState();
                    break;
            }
    }

    public override void OnRender()
    {

        if (gSkin)
        {
            GUI.skin = gSkin;
            GUI.skin.label.fontSize = 32;
            GUI.color = new Color(1, 0.36f, 0.22f, 1);

            GUI.Box(new Rect((Screen.width * .5f) - (Screen.width * .3f), (Screen.height * .5f) - (Screen.height * .35f),
                                                        (Screen.width * .6f), (Screen.height * .7f)), "");
            GUI.color = Color.white;
        }


        GUI.color = new Color(1, 0.36f, 0.22f, 1);
        GUI.Label(new Rect((Screen.width * .5f) - (Screen.width * .25f), (Screen.height * .5f), 400, 200), FullText);
        string jump = "\n";

        foreach (int Option in OptionsList.Keys)
        {

            if (Option == ChooseOption)
            {
                GUI.color = Color.white;
                GUI.Label(new Rect((Screen.width * .5f) - (Screen.width * .25f),
                        (Screen.height * .5f), 400, 200), jump + OptionsList[Option]);
            }

            jump += System.Environment.NewLine;
        }

        GUI.color = Color.white;
    }

    public override void Pause() { ;}
    public override void Resume() { ;}

}