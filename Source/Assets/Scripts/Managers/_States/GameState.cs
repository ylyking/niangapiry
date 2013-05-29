using UnityEngine;
using System.Collections;

public abstract class GameState : MonoBehaviour
{
    public abstract void Init();
    public abstract void DeInit();
	
    public abstract void Pause();
    public abstract void Resume();

    public abstract void OnUpdate();
    public virtual void OnRender() { ;}                 // A Render Helper
    public virtual uint Score {  get; set; }
	
    //public void ChangeState(System.Type newStateType) 
    //{
    //    Managers.Game.ChangeState(newStateType);
    //}
	
}