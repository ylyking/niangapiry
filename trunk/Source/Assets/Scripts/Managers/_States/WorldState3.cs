using UnityEngine;
using System.Collections;

public class WorldState3 : InGameState
{

    public override void Init()
    {
        //Debug.Log("Enter World 3 State: Iguazú");
        Managers.Tiled.Load(Managers.Register.IguazuFile);
        //Managers.Tiled.Load("/Levels/Iguazu2.tmx");
        base.Init();
    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.IguazuFile = Managers.Register.currentLevelFile;

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
        Debug.Log("Called Iguazu Resume!");

        if (Managers.Game.PlayerPrefab && !Managers.Game.IsPlaying)
        {
            Managers.Tiled.Load(Managers.Register.IguazuFile);

            Debug.Log("Reloaded Iguazu Level!");

            Managers.Game.IsPlaying = true;
            Managers.Game.IsPaused = false;
        }
    }
}
