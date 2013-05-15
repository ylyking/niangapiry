using UnityEngine;
using System.Collections;

public class WorldState4 : InGameState
{
    //public bool WorldFinished = false;
    //public int AoAoHealth = 15;

    public override void Init()
    {
        //Debug.Log("Enter World 4 State: SkyField");
        Managers.Tiled.Load(Managers.Register.SkyFieldFile);
        base.Init();

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");

        //if (WorldFinished)
        //    Managers.Register.SkyFieldFile = "/Levels/CampoDelCielo.tmx";   // Return Back to Main World (avoid AoAo boss Level)
        //else
            Managers.Register.SkyFieldFile = Managers.Register.currentLevelFile;

        Managers.Tiled.Unload();
        base.DeInit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void Pause()
    {

    }

    public override void Resume()
    {

    }
}
