using UnityEngine;
using System.Collections;


[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(ScreenManager))]
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(DataManager))]
[RequireComponent(typeof(TileManager))]

public class Managers : MonoBehaviour
{
    private static GameManager gameManager;
    public static GameManager Game
    {
        get { return gameManager; }
    }
 
    private static ScreenManager screenManager;
    public static ScreenManager Display
    {
        get { return screenManager; }
    }
 
    private static AudioManager audioManager;
    public static AudioManager Audio
    {
        get { return audioManager; }
    }

    private static DataManager dataManager;
    public static DataManager Register
    {
        get { return dataManager; }
    }
 
    private static TileManager tileManager;
    public static TileManager Tiled
    {
        get { return tileManager; }
    }

    private static ConversationManager conversationManager;
    public static ConversationManager Dialog
    {
        get { return conversationManager; }
    }
 
	    // Use this for initialization
    void Awake ()
    {
        //Find the references
        gameManager = GetComponentInChildren<GameManager>();
        screenManager = GetComponentInChildren<ScreenManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        dataManager = GetComponentInChildren<DataManager>();
        tileManager = GetComponentInChildren<TileManager>();
        conversationManager = GetComponentInChildren<ConversationManager>();

 
        //Make this game object persistant
        DontDestroyOnLoad(gameObject);
    }

}
