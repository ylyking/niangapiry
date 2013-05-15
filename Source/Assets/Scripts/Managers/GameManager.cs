using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public uint Score              = 0;
    public uint Fruits             = 0;
    public uint TopScore           = 100;

    //int FireGauge         = 0;
    //int Key               = 0;
    //public uint _Health   = 3;
    public uint Health      = 3;
    public uint Lifes       = 3;

    public bool ShowState   = true;
    public bool IsPlaying   = false;
    public bool IsPaused    = false;

    //////////////////////////////////////////////////////////////

    private List<GameState> states = new List<GameState>();
    private bool Running;

    public GameObject PlayerPrefab;
    public int UnlockedStages = 1;


    void Start()
    {
        //PlayerPrefab = GameObject.FindGameObjectWithTag("Player")as GameObject;
        if (!PlayerPrefab)
        {
            PlayerPrefab = gameObject;
            Debug.LogWarning("Player not in Scene, faking one");
        }

        //PushState(typeof(IntroState)); // Loading some State
        PushState(typeof(MainMenuState)); // A Starting ShortCut to menu

        UnlockedStages = PlayerPrefs.GetInt("UnlockedStages");
        if (UnlockedStages == 0)
            UnlockedStages = 1;
    }

    void OnApplicationQuit() 	// "DeInit()"
    {
        // cleanup the all states
        while (states.Count > 0)
        {
            states[states.Count - 1].DeInit();
            states.RemoveAt(states.Count - 1);
        }

        PlayerPrefs.SetInt("UnlockedStages", UnlockedStages);
    }

    void Update()
    {

        if ( !PlayerPrefab )
            PlayerPrefab = gameObject;                          // When Player it's not in Game Player not in Scene, fake one

        //if (Input.GetKeyDown("1") && this.states.Count == 0 )
        //    Managers.Game.PushState(typeof(WorldState0));                   // Pampero World

        //if (Input.GetKeyDown("2") && this.states.Count == 0)
        //    Managers.Game.PushState(typeof(WorldState1));                   // Monte World

        //if (Input.GetKeyDown("0") && this.states.Count == 0)
        //    Managers.Game.PushState(typeof(WorldState2));                   // Home World

        //if (Input.GetKeyDown("3") && this.states.Count == 0)
        //    Managers.Game.PushState(typeof(WorldState3));                   // Iguazu World

        //if (Input.GetKeyDown("4") && this.states.Count == 0)
        //    Managers.Game.PushState(typeof(WorldState4));                   // SkyField World

        //if (Input.GetKeyDown("5") && this.states.Count == 0)
        //    Managers.Game.PushState(typeof(WorldState5));                   // Impenetrable World

        //if (Input.GetKeyDown("u") && this.states.Count != 0)
        //    Managers.Game.PopState();                                       // UNload


        if (((Fruits % 100) == 0) && System.Convert.ToBoolean(Fruits))
        { 
            ShowState = true; 
            Lifes++; 
            Fruits++;
        }

        if (states.Count > 0)
            states[states.Count - 1].OnUpdate();
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

        //Managers.Display.ShowSynth( 0.25f);	

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

    //Changes the current game state after a specific time
    //public void SetState(System.Type newStateType, float Delay = 0)
    //{
    //    StartCoroutine(CloseLastState(newStateType, Delay));
    //}

    //private IEnumerator CloseLastState(System.Type newStateType, float Delay)
    //{
    //    yield return new WaitForSeconds(Delay);								// We must check the end of Button animation

    //    if (states.Count > 0)
    //        states[states.Count - 1].DeInit();

    //    states[states.Count - 1] = GetComponentInChildren(newStateType) as GameState;

    //    if (states.Count > 0)
    //        states[states.Count - 1].Init();
    //}
    //////////////////////////////////////////////////////////////

    public void GameOver()
    {
        IsPlaying = false;
        ResetStats();
        //ShowFlash(2.0f);

        //Application.LoadLevel("Intro");
        Managers.Game.PopState();
        Managers.Register.SoftReset();
    }
    //////////////////////////////////////////////////////////////

    public void ResetStats()
    {
        if (Score > TopScore)
            TopScore = Score;
        PlayerPrefs.SetInt("TopScore", (int)TopScore);

        Score = 0;
        Fruits = 0;

        //FireGauge = 0;
        //Key = 0;

        ShowState = false;
        Health = 3;
        Lifes = 3;
    }
    //////////////////////////////////////////////////////////////

    public void DeInit() { ;}
    //////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////     
    //public void ShowHelp()
    //{
    //    if (states.Count > 0)
    //        states[states.Count - 1].ShowHelp();
    //}

}

