using UnityEngine;
using System.Collections;

public abstract class IngameState : GameState		// Using Decorator Pattern to avoid GamePlay HUD things redundance 
{
	
    //protected uint 				score 		= 0;
    //public override uint 		Score	{ get { return this.score ; } set { this.score = value; }  }


    //protected float TimeDisplay = 0;
	
    //private Vector2 PausePos;
    //private Vector2 TimePos;
    //private Vector2 ScorePos;
    //private Vector2 AbductionPos;
	
    public override void Init()
	{
        //score = 0;

        //if ( (Managers.Display.HUDCorned) == true )
        //{
	
        //}
        //else
        //{
        //    //Debug.Log("Corned");
        //}	

	}
	
    public override void DeInit()
	{
		

	}
	
    public override void OnUpdate()
	{

	}
	
	public override void Pause()
	{
	
	}
	
	public override void Resume()
	{
	
	}
	
	public override void CheckScore()
	{
//        string MissionScore = (Managers.Game.State.GetType()).ToString();						// Get current Mission Top Score name
//        int MissionId = int.Parse( MissionScore.Substring( MissionScore.LastIndexOf("_")+ 1 ) );	 
//        MissionScore = "Top Score " + MissionId   ; 
////		Debug.Log("Loading" + MissionScore);
		
//        if( this.score > PlayerPrefs.GetInt( MissionScore ) )
//        {
//            PlayerPrefs.SetInt( MissionScore, (int)score );
//            GreatScore.text += this.score;
//            GreatScore.hidden = false;
//        }
	}
	
//	public virtual void ShowStatus()
//	{
//		PauseButton.hidden 	= false;
//		text1.hidden 		= false;
//		text2.hidden 		= false;
//		text3.hidden 		= false;
//	}
	
//	public virtual void LevelCompleted()
//	{
//		;
//	}	
//	
}