using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour
{

	private GameState currentStage;
	public int UnlockedStages = 2;
	
	 
    void Start()
    {
//        SetMission(typeof(Brief)); // Loading some State
        //Managers.Tiled.Load("/Resources/Untitled/Tiled.xml");
        
        //Managers.Tiled.Load("/Levels/Tiled.tmx");

        //Managers.Tiled.Load("/Resources/Test/level1.tmx");

        //Managers.Tiled.Load("/Levels/level1.tmx");    // < - - For Level Loader Deploy
 
    }
	
	void Update()
    {
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
