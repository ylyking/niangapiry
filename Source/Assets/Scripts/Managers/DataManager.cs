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

    //[SerializeField]
    public uint Score               = 0;
    public uint Fruits              = 0;
    public uint TopScore            = 100;

    public uint FireGauge           = 0;
    public int  Key                 = 0;
    public uint Health              = 3;
    public uint Lifes               = 3;

    private GameState currentStage;
    public int UnlockedStages       = 2;

    public string currentLevelFile = "";
    public Dictionary<string, Vector3> MapCheckPoints = new Dictionary<string, Vector3>();


    #region WORLD 0: PAMPERO

    public string PamperoFile = "/Levels/Pampero.tmx";                                          


    #endregion
    ////////////////////////////////////////////////////
    
                                                                                            // each World Last File Loaded
    #region WORLD 1: MONTE

    public string MonteFile = "/Levels/Monte.tmx";                                           


    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 2: HOME

    public string HomeFile = "/Levels/Home.tmx";

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 3: IGUAZU

    public string IguazuFile = "/Levels/Iguazu.tmx";


    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 4: SkyField

    public string SkyFieldFile = "/Levels/CampoDelCielo.tmx";


    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 5: IMPENETRABLE

    public string ImpenetrableFile = "/Levels/Impenetrable.tmx";


    #endregion
    ////////////////////////////////////////////////////    




    // Use this for initialization
	void Start ()
    {
        MapCheckPoints.Add("/Levels/Pampero.tmx"        , Vector3.zero);
        MapCheckPoints.Add("/Levels/Monte.tmx"          , Vector3.zero);
        MapCheckPoints.Add("/Levels/Iguazu.tmx"         , Vector3.zero);
        MapCheckPoints.Add("/Levels/CampoDelCielo.tmx"  , Vector3.zero);
        MapCheckPoints.Add("/Levels/Impenetrable.tmx"   , Vector3.zero);



	}


    // Player setup inside level position
    public void SetPlayerPos()
    {
        if ( !MapCheckPoints.ContainsKey(currentLevelFile))                         // If our current file isn't registered yet
             MapCheckPoints.Add(currentLevelFile, Vector3.zero);

        //1) If there's a previous saved position use it
        if (MapCheckPoints[currentLevelFile] != Vector3.zero)
            Managers.Game.PlayerPrefab.transform.position = MapCheckPoints[currentLevelFile];

        //2) else search a new one with a special spot in scene
        else if (GameObject.FindGameObjectWithTag("Respawn"))
        {
            Portal[] portals = (Portal[])GameObject.FindSceneObjectsOfType(typeof(Portal));
            for (int portalIndex = portals.Length - 1; portalIndex >= 0; portalIndex--)
            {
                //2.a)  if there is a "start" Id, go there
                if (portals[portalIndex].Id.ToLower() == "start")
                {
                    Managers.Display.ShowFlash(0.5f);
                    MapCheckPoints[currentLevelFile] = portals[portalIndex].gameObject.transform.position;
                    break;
                }
            }
        }

        //3) else if still Vector3.zero, auto-setup one position by default
        if (MapCheckPoints[currentLevelFile] == Vector3.zero)
            MapCheckPoints[currentLevelFile] = new Vector3(Managers.Display.cameraScroll.GetBounds().xMin + 1,
                                                        Managers.Display.cameraScroll.GetBounds().yMax - 1, 0);

        //4) Finally Go There !
        Managers.Game.PlayerPrefab.transform.position = MapCheckPoints[currentLevelFile];
   
    }                                           


    void SoftReset()
    {
        Fruits = 0;
        Key = 0;
        Health = 3;
        Lifes = 3;
    }

    void HardReset()
    {
        Score = 0;
        Fruits = 0;
        TopScore = 100;

        FireGauge = 0;
        Key = 0;
        Health = 3;
        Lifes = 3;

        //currentStage;
        UnlockedStages  = 2;

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