using UnityEngine;
using System.Collections;

public class WorldState3 : InGameState
{
    public override void Init()
    {
        //Debug.Log("Enter World 3 State: Iguazú");
        Managers.Register.StartPoint = Managers.Register.IguazuStart;
        Managers.Tiled.Load(Managers.Register.IguazuFile);
        base.Init();
    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.IguazuStart = Managers.Register.StartPoint;
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

    }
}
