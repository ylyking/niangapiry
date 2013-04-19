using UnityEngine;
using System.Collections;

public class WorldState5 : InGameState
{
    public override void Init()
    {
        //Debug.Log("Enter World 5 State: Impenetrable");
        Managers.Register.StartPoint = Managers.Register.ImpenetrableStart;
        Managers.Tiled.Load(Managers.Register.ImpenetrableFile);
        base.Init();

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.ImpenetrableStart = Managers.Register.StartPoint;
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

    }
}
