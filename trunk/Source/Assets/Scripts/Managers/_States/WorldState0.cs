using UnityEngine;
using System.Collections;

public class WorldState0 : InGameState
{
    public override void Init()
    {
        //Debug.Log("Enter World 0 State: Pampero");
        base.Init();
       Managers.Tiled.Load("/Levels/pampero.tmx") ;
        //if ( Managers.Tiled.Load("/Levels/parallax.tmx") )
            //(Managers.Display.MainCamera).GetComponent<CameraScrolling>().ResetBounds();
        //else
        //    Debug.Log( " level map Not found");
    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
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
