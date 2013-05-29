using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class DataManager : MonoBehaviour 
{

/// <summary>
/// COMMON GAMEPLAY DATA FIELDS
/// Here We have the most important ingame properties
/// </summary>

    public int Score        = 0;
    public int Fruits       = 0;
    public int TopScore     = 100;

    public int  FireGauge   = 0;
    public int  Key         = 0;
    public int Health       = 3;
    public int Lifes        = 3;

    public enum Items { Empty = 0, Hat = 1, Whistler = 2, Invisibility = 4, Smallibility = 8, Fire = 16 };
    public Items Inventory = Items.Empty;					// Inventory system activation

    private GameState currentStage;
    //[HideInInspector]
    public int UnlockedStages = 3;

    public string currentLevelFile = "";
    public string previousLevelFile = "";
    public Dictionary<string, Vector3> MapCheckPoints = new Dictionary<string, Vector3>();

    public bool PlayerAutoRunning = true;

    // each World Last File Loaded

    #region WORLD 0: PAMPERO

    public string PamperoFile = "/Levels/Pampero.tmx";                                          

    #endregion
    ////////////////////////////////////////////////////
    

    #region WORLD 1: MONTE
    public string MonteFile = "/Levels/Monte.tmx";                                           

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 2: HOME
    public bool Souvenir1 = false;
    public bool Souvenir2 = false;
    public bool Souvenir3 = false;
    public bool Souvenir4 = false;
    public bool Souvenir5 = false;

    public string HomeFile = "/Levels/Home.tmx";

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 3: IGUAZU

    public string IguazuFile = "/Levels/Iguazu.tmx";


    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 4: SkyField
    public string SkyFieldFile = "/Levels/CampoDelCielo.tmx";
    public bool AoAoDefeated = false;

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 5: IMPENETRABLE

    public string ImpenetrableFile = "/Levels/Impenetrable.tmx";


    #endregion
    ////////////////////////////////////////////////////    




    // Use this for initialization
	void Start ()
    {
        MapCheckPoints.Add(PamperoFile          , Vector3.zero);
        MapCheckPoints.Add(MonteFile            , Vector3.zero);
        MapCheckPoints.Add(IguazuFile           , Vector3.zero);
        MapCheckPoints.Add(SkyFieldFile         , Vector3.zero);
        MapCheckPoints.Add(ImpenetrableFile     , Vector3.zero);
	}


    // Player setup inside level position
    public void SetPlayerPos()
    {
        if (!MapCheckPoints.ContainsKey(currentLevelFile))                      // If our current file isn't registered yet
            MapCheckPoints.Add(currentLevelFile, Vector3.zero);

                                                                                // If there's a previous saved position use it
        if (MapCheckPoints[currentLevelFile] != Vector3.zero)
            Managers.Game.PlayerPrefab.transform.position = MapCheckPoints[currentLevelFile];
        else
            MapCheckPoints[currentLevelFile] = Managers.Game.PlayerPrefab.transform.position;
  
    }                                           


    public void SoftReset()
    {
        Debug.Log("Doing Soft Reset");

        Fruits = 0;
        Key = 0;
        Health = 3;
        Lifes = 3;

        //FireGauge = 0;
        //Inventory = Items.Empty;
    }

    public void HardReset()
    {
        Score = 0;
        Fruits = 0;
        TopScore = 100;

        FireGauge = 0;
        Key = 0;
        Health = 3;
        Lifes = 3;
        Inventory = Items.Empty;

        Souvenir1 = false;
        Souvenir2 = false;
        Souvenir3 = false;
        Souvenir4 = false;
        Souvenir5 = false;


        //currentStage;
        UnlockedStages = 3;

        HomeFile        = "/Levels/Home.tmx";
        MonteFile       = "/Levels/Monte.tmx";
        PamperoFile     = "/Levels/Pampero.tmx";
        IguazuFile      = "/Levels/Iguazu.tmx";
        SkyFieldFile    = "/Levels/CampoDelCielo.tmx";
        ImpenetrableFile = "/Levels/Impenetrable.tmx";

        MapCheckPoints.Clear();

    }


    public void Save(string name)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(DataManager));
        using (var ms = new MemoryStream())
        {
            serializer.Serialize(ms, Managers.Register);
            PlayerPrefs.SetString(name, System.Text.ASCIIEncoding.ASCII.GetString(ms.ToArray()));
        }
    }

    public void Load(string name)
    {
        DataManager instance;

        if (!PlayerPrefs.HasKey(name)) instance = default(DataManager);
        XmlSerializer serializer = new XmlSerializer(typeof(DataManager));
        using (var ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(PlayerPrefs.GetString(name))))
        {
            instance = (DataManager)serializer.Deserialize(ms);
        }

        Managers.Register.UnlockedStages = instance.UnlockedStages;
        Managers.Register.Score = instance.Score;
        Managers.Register.TopScore = instance.TopScore;

        Managers.Register.Fruits = instance.Fruits;
        Managers.Register.Lifes = instance.Lifes;
        Managers.Register.Health = instance.Health;

        Managers.Register.Inventory = instance.Inventory;

        Managers.Register.MapCheckPoints    = instance.MapCheckPoints;
        Managers.Register.MonteFile         = instance.MonteFile;
        Managers.Register.PamperoFile       = instance.PamperoFile;
        Managers.Register.IguazuFile        = instance.IguazuFile;
        Managers.Register.SkyFieldFile      = instance.SkyFieldFile;
        Managers.Register.ImpenetrableFile  = instance.ImpenetrableFile;

    }
}












//Store objects in PlayerPrefs.
//Usage:
//var myObject = new MyClass();
//Prefs.Save<MyClass>("my object", myObject);
//var anotherObject = Prefs.Load<MyClass>("my object")


//or something like that...

//using UnityEngine;
//using System.Collections;
//using System.Xml.Serialization;
//using System.IO;


//public class Prefs
//{

//    public static void Save<T> (string name, T instance)
//    {
//        XmlSerializer serializer = new XmlSerializer (typeof(T));
//        using (var ms = new MemoryStream ()) {
//            serializer.Serialize (ms, instance);
//            PlayerPrefs.SetString (name, System.Text.ASCIIEncoding.ASCII.GetString (ms.ToArray ()));
//        }
//    }

//    public static T Load<T> (string name)
//    {
//        if(!PlayerPrefs.HasKey(name)) return default(T);
//        XmlSerializer serializer = new XmlSerializer (typeof(T));
//        T instance;
//        using (var ms = new MemoryStream (System.Text.ASCIIEncoding.ASCII.GetBytes (PlayerPrefs.GetString (name)))) {
//            instance = (T)serializer.Deserialize (ms);
//        }
//        return instance;
//    }
    
//}