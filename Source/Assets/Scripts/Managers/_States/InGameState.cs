using UnityEngine;
using System.Collections;

public abstract class InGameState : GameState {

    public override void Init()
    {
        Managers.Game.PlayerPrefab = (GameObject)Instantiate(Resources.Load("Prefabs/Pombero", typeof(GameObject)));
        Managers.Game.PlayerPrefab.name = "Pombero";
        Managers.Display.cameraScroll.SetTarget(Managers.Game.PlayerPrefab.transform, false);

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        //Managers.Tiled.Unload();
        var Player = Managers.Game.PlayerPrefab;
        if (Player == null) 
            return;

        //Managers.Game.PlayerPrefab = Managers.Game.gameObject;
        //CameraScrolling cam = (CameraScrolling)(Managers.Display.MainCamera).GetComponent<CameraScrolling>();
        //cam.SetTarget( Managers.Display.transform, false);

        //Managers.Display.cameraScroll.SetTarget(Managers.Display.transform, false);
        if (Player)
            Destroy(Player);

    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown("escape"))
            Managers.Game.PushState(typeof(PauseState));
    }

    public override void OnRender()
    {
        ;
    }

    public override void Pause()
    {

    }

    public override void Resume()
    {

    }
}
