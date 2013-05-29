using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreditsMenuState : GameState 
{
    string FullText = "Volver";
    string InfoText = "\n\n\n\n Programado By CarliXYZ \n Arte & Diseño By FAC! \n Music By Fernando Verga & Menta-Beat \n\n ÑANGAPIRY 86® - 2012  \n\n- SUNHOUSE™ - ";

    GameObject IntroPrefab;
    GUISkin gSkin;

    public override void Init()
    {
        gSkin = (GUISkin)Resources.Load("GUI/GUISkin B", typeof(GUISkin));
        FullText = "\n\n\n\nVolver";
        InfoText = "\n\n\n\n Programado By CarliXYZ \n Arte & Diseño By FAC! \n Music By Fernando Vega & Menta-Beat \n\n ÑANGAPIRY 86® - 2012  \n\n- SUNHOUSE™ - ";
    }
	

    public override void DeInit()
    {
        FullText = "";
        InfoText = "";
    }
	
    public override void OnUpdate()
    {

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetKeyDown("escape"))
        {
            Managers.Game.PopState();
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
                                                        (Screen.width * .6f), (Screen.height * .7f)), InfoText);
            GUI.color = Color.white;
        }


        //GUI.color = new Color(1, 0.36f, 0.22f, 1);
        GUI.Label(new Rect((Screen.width * .5f) - (Screen.width * .25f), (Screen.height * .5f), 400, 200), FullText);

        GUI.color = Color.white;

    }

    public override void Pause() { ;}
    public override void Resume() { ;}
}