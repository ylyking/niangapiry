using UnityEngine;
using System.Collections;

public abstract class InGameState : GameState {

    public override void Init()
    {
        Managers.Game.PlayerPrefab = (GameObject)Instantiate(Resources.Load("Prefabs/Pombero", typeof(GameObject)));
        Managers.Game.PlayerPrefab.name = "Pombero";
        Managers.Display.cameraScroll.SetTarget(Managers.Game.PlayerPrefab.transform, false);

        Managers.Register.SetPlayerPos();
    }

    public override void DeInit()
    {
        var Player = Managers.Game.PlayerPrefab;
        if (Player == null) 
            return;

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
