using UnityEngine;
using System.Collections;

public abstract class InGameState : GameState {

    public override void Init()
    {
        //Managers.Game.PlayerPrefab = null;
        Managers.Game.PlayerPrefab = (GameObject)Instantiate(Resources.Load("Prefabs/Pombero", typeof(GameObject)));
        Managers.Game.PlayerPrefab.name = "Pombero";
        Managers.Display.cameraScroll.SetTarget(Managers.Game.PlayerPrefab.transform, false);
        Managers.Tiled.PlayerTransform = Managers.Game.PlayerPrefab.transform;

        Debug.Log("setting up position in TileManager");
        Managers.Register.SetPlayerPos();

        Managers.Game.IsPlaying = true;
    }

    public override void DeInit()
    {
        var Player = Managers.Game.PlayerPrefab;
        if (Player == null) 
            return;

        if (Player)
            Destroy(Player);

        Managers.Game.IsPlaying = false;

    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown("escape"))
            Managers.Game.PushState(typeof(PauseState));
    }

    public override void OnRender()
    {
        // I Could setup Here all The ShowStatus thing, but no, thanks
        ;
    }

    public override void Pause()
    {

    }

    public override void Resume()
    {

    }
}
