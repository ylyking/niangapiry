using UnityEngine;
using System.Collections;

public abstract class GameState : MonoBehaviour
{
    public abstract void Init();
    public abstract void DeInit();
	
    public abstract void Pause();
    public abstract void Resume();
	
    public abstract void OnUpdate();
    public virtual void ShowHelp(){ Debug.Log("Que Dios te ayude"); }
    public virtual uint Score {  get; set; }
    public virtual void CheckScore() {  Debug.Log("No Score to check here");  }
	
	public void ChangeState(System.Type newStateType) 
	{
		Managers.Game.ChangeState(newStateType);
	}
	
}