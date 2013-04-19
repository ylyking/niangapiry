using UnityEngine;
using System.Collections;
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
    public Vector3 StartPoint = Vector3.zero;


    #region WORLD 0: PAMPERO

    public string PamperoFile = "/Levels/Pampero.tmx";
    public Vector3 PamperoStart = Vector3.zero;

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 1: MONTE

    public string MonteFile = "/Levels/Monte.tmx";                                          // World Last File Loaded
    public Vector3 MonteStart = Vector3.zero;                                               // Level last position Loaded
    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 2: HOME

    public string HomeFile = "/Levels/Home.tmx";
    public Vector3 HomeStart = Vector3.zero;

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 3: IGUAZU

    public string IguazuFile = "/Levels/Iguazu.tmx";
    public Vector3 IguazuStart = Vector3.zero;

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 4: IGUAZU

    public string SkyFieldFile = "/Levels/CampoDelCielo.tmx";
    public Vector3 SkyFieldStart = Vector3.zero;

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 5: IMPENETRABLE

    public string ImpenetrableFile = "/Levels/Impenetrable.tmx";
    public Vector3 ImpenetrableStart = Vector3.zero;

    #endregion
    ////////////////////////////////////////////////////    




    // Use this for initialization
	void Start ()
    {
	
	}


    // Player setup inside level position
    public void SetPlayerPos()
    {
    //1) If there's a previous saved position use it
        if (Managers.Register.StartPoint != Vector3.zero)                   
            Managers.Game.PlayerPrefab.transform.position = Managers.Register.StartPoint;

    //2) else search one special spot in scene
        else if (GameObject.FindGameObjectWithTag("Respawn"))               
        {
            Portal[] portals = (Portal[])GameObject.FindSceneObjectsOfType(typeof(Portal));
            for (int portalIndex = portals.Length - 1; portalIndex >= 0; portalIndex--)
            {
    //2.a)  if there is a "start" Id, go there
                if (portals[portalIndex].Id.ToLower() == "start")           
                {
                    Managers.Display.ShowFlash(0.5f);
                    Managers.Register.StartPoint = portals[portalIndex].gameObject.transform.position;
                    break;
                }
            }

            //Debug.Log("There isn't start Point in the Map, setup the first spot found");
    //2.b) else use the first one found
            //Managers.Register.StartPoint = portals[0].transform.position;  
        }

    //3) else auto-setup one position by default
        if (Managers.Register.StartPoint == Vector3.zero)
            Managers.Register.StartPoint = new Vector3(Managers.Display.cameraScroll.GetBounds().xMin + 1,
                                                        Managers.Display.cameraScroll.GetBounds().yMax - 1, 0);

    //4) Finally Go There !
        Managers.Game.PlayerPrefab.transform.position = Managers.Register.StartPoint;
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
        UnlockedStages       = 2;

        HomeFile = "/Levels/Home.tmx";
        MonteFile = "/Levels/Monte.tmx";
        PamperoFile = "/Levels/Pampero.tmx";
        IguazuFile = "/Levels/Iguazu.tmx";
        SkyFieldFile = "/Levels/CampoDelCielo.tmx";
        ImpenetrableFile = "/Levels/Impenetrable.tmx";

        ImpenetrableStart = SkyFieldStart = IguazuStart = PamperoStart = MonteStart = HomeStart = Vector3.zero;

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


        Managers.Register.MonteFile = instance.MonteFile;
        Managers.Register.MonteStart = instance.MonteStart;

        Managers.Register.PamperoFile = instance.PamperoFile;
        Managers.Register.PamperoStart = instance.PamperoStart;

        Managers.Register.IguazuFile = instance.IguazuFile;
        Managers.Register.IguazuStart = instance.IguazuStart;

        Managers.Register.SkyFieldFile = instance.SkyFieldFile;
        Managers.Register.SkyFieldStart = instance.SkyFieldStart;

        Managers.Register.ImpenetrableFile = instance.ImpenetrableFile;
        Managers.Register.ImpenetrableStart = instance.ImpenetrableStart;
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