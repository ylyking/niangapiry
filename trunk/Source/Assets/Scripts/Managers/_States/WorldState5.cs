using UnityEngine;
using System.Collections;

public class WorldState5 : InGameState
{
    public override void Init()
    {
        //Debug.Log("Enter World 5 State: Impenetrable");
        Managers.Tiled.Load(Managers.Register.ImpenetrableFile);
        base.Init();

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.ImpenetrableFile = Managers.Register.currentLevelFile;

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
        Debug.Log("Called Impenetrable Resume!");

        if (Managers.Game.PlayerPrefab && !Managers.Game.IsPlaying)
        {
            Managers.Tiled.Load(Managers.Register.ImpenetrableFile);

            Debug.Log("Reloaded Impenetrable Level!");

            Managers.Game.IsPlaying = true;
            Managers.Game.IsPaused = false;
        }
    }
}
