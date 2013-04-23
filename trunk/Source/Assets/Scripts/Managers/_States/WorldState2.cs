using UnityEngine;
using System.Collections;

public class WorldState2 : InGameState
{
    public override void Init()
    {
        //Debug.Log("Enter World 2 State: Pombero's House");
        Managers.Tiled.Load(Managers.Register.HomeFile);
        base.Init();

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.HomeFile = Managers.Register.currentLevelFile;

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
