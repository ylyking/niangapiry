using UnityEngine;

public class WorldState2 : InGameState
{
	public override void Init()
	{
		Managers.Game.IsPlaying = true;
		if (Managers.Register.YaguaDefeated && Managers.Register.MonaiDefeated && !Managers.Register.YasiYatereDefeated)
		{
			if (Managers.Register.UnlockedStages <= 3)
				Managers.Register.UnlockedStages = 4;
			Managers.Tiled.Load("Home2");
		}
		else if (Managers.Register.AoAoDefeated)
		{
			if (Managers.Register.UnlockedStages <= 5)
				Managers.Register.UnlockedStages = 6;
			Managers.Tiled.Load("Home4");
		}
		else if (Managers.Register.YasiYatereDefeated)
		{
			if (Managers.Register.UnlockedStages <= 4)
				Managers.Register.UnlockedStages = 5;
			Managers.Tiled.Load("Home3");
		}
		else if (Managers.Register.PlayTutorialFirst)
		{
			Managers.Register.PlayTutorialFirst = false;
			this.LoadWorld("Tutorial");
		}
		else
			Managers.Tiled.Load("Home1");
		Managers.Display.camTransform.position = new Vector3(3.75f, 2.5f, 0.0f);
		base.Init();
	}
	
	public override void DeInit()
	{
		Managers.Register.HomeFile = Managers.Register.currentLevelFile;
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
		if (!(Managers.Register.currentLevelFile == string.Empty))
			return;
		Managers.Display.camTransform.position = new Vector3(3.75f, 2.5f, -2.5f);
		Managers.Tiled.Load("Home1");
	}
	
	private void LoadWorld(string world)
	{
		Managers.Display.ShowFlash(1f);
		Managers.Tiled.Unload();
		if (!Managers.Tiled.Load(world))
			return;
		Managers.Game.PushState(typeof (DummyState));
		Input.ResetInputAxes();
	}
}


//using UnityEngine;
//using System.Collections;
//
//public class WorldState2 : InGameState
//{
//	public AudioClip MusicMap;
//	
//    public override void Init()
//    {
//        Managers.Game.IsPlaying = true;
//		
////		Managers.Audio.PlayMusic( MusicMap, 1, 1);
//
//        if (Managers.Register.YaguaDefeated && Managers.Register.MonaiDefeated && !Managers.Register.YasiYatereDefeated)
//        {
//            if (Managers.Register.UnlockedStages <= 3)
//                Managers.Register.UnlockedStages = 4;
//            Managers.Tiled.Load("Home2");
//        }
//        else if (Managers.Register.YasiYatereDefeated)
//        {
//            if (Managers.Register.UnlockedStages <= 4)
//                Managers.Register.UnlockedStages = 5;
//            Managers.Tiled.Load("Home3");
//        }
//        else
//            Managers.Tiled.Load("Home1");
//
//        Managers.Display.camTransform.position = new Vector3(3.75f, 2.5f, 0);
//
//        base.Init();
//
//    }
//
//    public override void DeInit()
//    {
//        //Debug.Log("Exit the current State and returning map");
//        Managers.Register.HomeFile = Managers.Register.currentLevelFile;
//
//        Managers.Tiled.Unload();
//        base.DeInit();
//    }
//
//    public override void OnUpdate()
//    {
//        base.OnUpdate();
//
//    }
//
//    public override void Pause()
//    {
//
//    }
//
//    public override void Resume()
//    {
//
//    }
//}
