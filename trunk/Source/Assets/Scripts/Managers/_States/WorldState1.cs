using UnityEngine;
using System.Collections;

public class WorldState1 : InGameState
{
    public override void Init()
    {
        Debug.Log("Enter World 1 State: Monte");
        Managers.Tiled.Load(Managers.Register.MonteFile);
        base.Init();

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.MonteFile = Managers.Register.currentLevelFile;

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
