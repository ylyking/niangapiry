using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour
{

	private GameState currentStage;
	public int UnlockedStages = 2;

    //public LevelAttributes currentLevel;
	
	 
    void Start()
    {
//        SetMission(typeof(Brief)); // Loading some State
        //Managers.Tiled.Load("/Resources/Untitled/Tiled.xml");

        //Managers.Tiled.Load("/Levels/Tiled.tmx");

        //Managers.Tiled.Load("/Resources/Test/level1.tmx");

        //Managers.Tiled.Load("/Levels/level1.tmx");    // < - - For Level Loader Deploy

        Managers.Tiled.Load("/Levels/home.tmx");    // < - - For Scroll Parallax Loader Deploy



 
    }
	
	void Update()
    {
        if (Input.GetKeyUp("0"))
            //Managers.Tiled.Load("/Levels/parallax.tmx");
            Managers.Tiled.Load("/Levels/home.tmx");   
        if (Input.GetKeyUp("1"))
            Managers.Tiled.Load("/Levels/level1.tmx");
        if (Input.GetKeyUp("2"))
            Managers.Tiled.Load("/Levels/level2.tmx");
        if (Input.GetKeyUp("3"))
            Managers.Tiled.Load("/Levels/level3.tmx");
        if (Input.GetKeyUp("4"))
            Managers.Tiled.Load("/Levels/level4.tmx");
        if (Input.GetKeyUp("5"))
            Managers.Tiled.Load("/Levels/level5.tmx");    

        if (Input.GetKeyUp("u"))
            Managers.Tiled.Unload();

    
        if (currentStage != null)
            currentStage.OnUpdate();
    }
	
    public GameState Current
    {
        get { return currentStage; }
    }
 
    //Changes the current game state
    public void SetStage(System.Type newStateType)
    {
        if (currentStage != null)
            currentStage.DeInit();
 
        currentStage = GetComponentInChildren(newStateType) as GameState;
		
        if (currentStage != null)
            currentStage.Init();
    }
	
	public void ResetStage()
	{
		if (currentStage == null)
		{ 
			print ("No Mission Loading");
			return;
		}
		
            currentStage.DeInit();
            currentStage.Init();
	}
	
}
