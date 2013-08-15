using UnityEngine;
using System.Collections;

public class DummyState : InGameState
{
    public override void Init()
    {
        //Managers.Game.IsPaused = false;

        base.Init();

    }

    public override void DeInit()
    {
        //Managers.Game.IsPaused = true;

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
