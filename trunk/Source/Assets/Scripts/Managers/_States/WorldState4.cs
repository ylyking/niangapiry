using UnityEngine;
using System.Collections;

public class WorldState4 : InGameState
{
    public override void Init()
    {
        //Debug.Log("Enter World 4 State: SkyField");
        Managers.Register.StartPoint = Managers.Register.SkyFieldStart;
        Managers.Tiled.Load(Managers.Register.SkyFieldFile);
        base.Init();

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.PamperoStart = Managers.Register.StartPoint;
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
