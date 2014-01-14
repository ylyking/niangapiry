using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public bool ShowState   = true;
    public bool IsPlaying   = false;
    public bool IsPaused    = false;

    //////////////////////////////////////////////////////////////

    private List<GameState> states = new List<GameState>();
    private bool Running;

    public GameObject PlayerPrefab;


    void Start()
    {
        //PushState(typeof(IntroState)); // Loading some State
        PushState(typeof(MainMenuState)); // A Starting ShortCut to menu
    }
	
 
	
    void OnApplicationQuit() 	// "DeInit()"
    {
        // cleanup the all states
        while (states.Count > 0)
        {
            states[states.Count - 1].DeInit();
            states.RemoveAt(states.Count - 1);
        }

        //PlayerPrefs.SetInt("UnlockedStages", UnlockedStages);
    }

    void Update()
    {
        Managers.Register.Health = Mathf.Clamp(Managers.Register.Health, 0, 3);

        if (((Managers.Register.Fruits % 85) == 0) && System.Convert.ToBoolean(Managers.Register.Fruits))
        { 
            ShowState = true;
            Managers.Register.Lifes++;
            Managers.Register.Fruits++;
        }

        if (states.Count > 0)
            states[states.Count - 1].OnUpdate();


        //if (Input.GetKeyDown("t"))
        //    Managers.Game.PushState(typeof(ScoreInput));


        //if (IsPlaying)
        //{
        //    Managers.Register.TimeDemo -= Time.deltaTime;


        //    if (Managers.Register.TimeDemo <= 0)
        //        Managers.Game.GameOver();
        //}
    }

    public void Render()
    {
        if (states.Count > 0)
            states[states.Count - 1].OnRender();
    }

    public void ChangeState(System.Type newStateType)		// Swap two states
    {
        // if not Empty CleanUp current State
        if (states.Count > 0)
        {
            states[states.Count - 1].DeInit();
            states.RemoveAt(states.Count - 1);
        }

        //Managers.Display.ShowSynth( 0.05f);	

        // store and init the new state
        states.Add(GetComponentInChildren(newStateType) as GameState);
        states[states.Count - 1].Init();
    }

    public void PushState(System.Type newStateType)		// Hold back previous states
    {
        // pause current state
        if (states.Count > 0)
            states[states.Count - 1].Pause();


        // store and init the new state
        states.Add(GetComponentInChildren(newStateType) as GameState);
        states[states.Count - 1].Init();
    }

    public void PopState()
    {
        // cleanup the current state
        if (states.Count > 0)
        {
            states[states.Count - 1].DeInit();
            states.RemoveAt(states.Count - 1);
        }

        //Managers.Display.ShowSynth( 0.05f);	

        // resume previous state
        if (states.Count > 0)
            states[states.Count - 1].Resume();
    }

    public GameState State
    {
        get { return states[states.Count - 1]; }

        //if (Managers.Game.State ==  Managers.Game.GetComponentInChildren< ExampleState >()  ) // Metodo para chequear 
        //    Debug.Log("Si, Soy el estado que estás buscando!");
        //    Debug.Log(" y aqui accedes a un dato del Estato:" + (( ExampleState )Managers.Game.State).PublicData);
    }
    
    //////////////////////////////////////////////////////////////

    public void DeInit() { ;}
    //////////////////////////////////////////////////////////////

    bool ToggleUp = true;
    public bool InputUp                             // This it's a little oneShot Up Axis check for doors & like   
    {
        get
        {
            if (Input.GetAxisRaw("Vertical") != 1)                      // It's like an "Input.GetAxisDown" 
                ToggleUp = true;

            if (ToggleUp && Input.GetAxisRaw("Vertical") >= 1)
            {
                ToggleUp = false;
                return true;
            }
            return false;
        }
    }

    bool ToggleDown = true;
    public bool InputDown                             // This it's a little oneShot Down Axis check for doors & like   
    {
        get
        {
            if (Input.GetAxisRaw("Vertical") != -1)                      // It's like an "Input.GetAxisDown" 
                ToggleDown = true;

            if (ToggleDown && Input.GetAxisRaw("Vertical") <= -1)
            {
                ToggleDown = false;
                return true;
            }
            return false;
        }
    }

    bool ToggleLeft = true;
    public bool InputLeft                             // This it's a little oneShot Left Axis check for doors & like   
    {
        get
        {
            if (Input.GetAxisRaw("Horizontal") != -1)                      // It's like an "Input.GetAxisDown" 
                ToggleLeft = true;

            if (ToggleLeft && Input.GetAxisRaw("Horizontal") <= -1)
            {
                ToggleLeft = false;
                return true;
            }
            return false;
        }
    }

    bool ToggleRight = true;
    public bool InputRight                             // This it's a little oneShot RIght Axis check for doors & like   
    {
        get
        {
            if (Input.GetAxisRaw("Horizontal") != 1)                      // It's like an "Input.GetAxisDown" 
                ToggleRight = true;

            if (ToggleRight && Input.GetAxisRaw("Horizontal") >= 1)
            {
                ToggleRight = false;
                return true;
            }
            return false;
        }
    }

}

