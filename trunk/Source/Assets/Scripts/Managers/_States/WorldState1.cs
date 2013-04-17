using UnityEngine;
using System.Collections;

public class WorldState1 : InGameState
{
    public override void Init()
    {
        Debug.Log("Enter World 1 State: Monte");
        base.Init();
        Managers.Tiled.Load("/Levels/world1.tmx");

        //if( Managers.Tiled.Load("/Levels/world1.tmx") )
        //   (Managers.Display.MainCamera).GetComponent<CameraScrolling>().ResetBounds();
        //else
        //    Debug.Log( " level map Not found");

    }

    public override void DeInit()
    {
        Debug.Log("Exit the current State and returning map");
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
