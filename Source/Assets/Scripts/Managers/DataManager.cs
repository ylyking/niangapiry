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

    //public float TimeDemo = 300;

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


    //public List<int> TopScore = new List<int>();
    //public Dictionary<int, string> TopScorePlayers = new Dictionary<int, string>();

    public bool FirstTimePlay       = true;
    public bool PlayerAutoRunning = true;

    // each World Last File Loaded

    #region WORLD 0: PAMPERO

    public string PamperoFile = "/Levels/Pampero.tmx";                                          
    public bool MonaiDefeated = false;

    #endregion
    ////////////////////////////////////////////////////
    

    #region WORLD 1: MONTE
    public string MonteFile = "/Levels/Monte.tmx";
    public bool YaguaDefeated = false;

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 2: HOME
    public bool Treasure1 = false;  // Mate
    public bool Treasure2 = false;  // Music
    public bool Treasure3 = false;  // Concepts Arts
    public bool Treasure4 = false;  // Inter-Dimensional Devices
    public bool Treasure5 = false;  // Comics

    public string HomeFile = "/Levels/Home.tmx";

    #endregion
    ////////////////////////////////////////////////////


    #region WORLD 3: IGUAZU

    public string IguazuFile = "/Levels/Iguazu.tmx";
    public bool MboiTuiDefeated = false;
    public bool YasiYatereDefeated = false;
    public bool MagicBlockEnabled = true;

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

        //for (int i = 1; i < 10; i++)
        //{
        //    TopScore.Add( i * 100);
            
        //    string name = "";
        //    switch (i)
        //    {
        //        case 9:
        //            name = "Horacio Guarani     ";
        //            break;
        //        case 8:
        //            name = "Oscar Aleman        ";
        //            break;
        //        case 7:
        //            name = "Roman Riquelme      ";
        //            break;
        //        case 6:
        //            name = "Diego Maradona      ";
        //            break;
        //        case 5:
        //            name = "Florencia De la V   ";
        //            break;
        //        case 4:
        //            name = "Jorge Coscia        ";
        //            break;
        //        case 3:
        //            name = "Alejandro Fantino   ";
        //            break;
        //        case 2:
        //            name = "Yasi Yatere         ";
        //            break;
        //        case 1:
        //            name = "Carlos Bianchi      ";
        //            break;
        //    }
        //    TopScorePlayers.Add(i * 100, name);
        //}
        //Managers.Register.TopScore.Sort();
        //Managers.Register.TopScore.Reverse();
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

        //ShowState = false;

        FireGauge = 0;
        Score = 0;
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
        //TopScore = 100;

        FireGauge = 0;
        Key = 0;
        Health = 3;
        Lifes = 3;
        Inventory = Items.Empty;

        Treasure1 = false;
        Treasure2 = false;
        Treasure3 = false;
        Treasure4 = false;
        Treasure5 = false;
        MagicBlockEnabled = true;

        //currentStage;
        UnlockedStages = 3;

        HomeFile        = "/Levels/Home.tmx";
        MonteFile       = "/Levels/Monte.tmx";
        PamperoFile     = "/Levels/Pampero.tmx";
        IguazuFile      = "/Levels/Iguazu.tmx";
        SkyFieldFile    = "/Levels/CampoDelCielo.tmx";
        ImpenetrableFile = "/Levels/Impenetrable.tmx";

        MapCheckPoints.Clear();

        YaguaDefeated = false;
        MonaiDefeated = false;
        MboiTuiDefeated = false;
        YasiYatereDefeated = false;
        AoAoDefeated = false;

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

        Managers.Register.Treasure1 = instance.Treasure1;
        Managers.Register.Treasure2 = instance.Treasure2;
        Managers.Register.Treasure3 = instance.Treasure3;
        Managers.Register.Treasure4 = instance.Treasure4;
        Managers.Register.Treasure5 = instance.Treasure5;

        Managers.Register.MagicBlockEnabled = instance.MagicBlockEnabled;

        Managers.Register.YaguaDefeated = instance.YaguaDefeated ;
        Managers.Register.MonaiDefeated = instance.MonaiDefeated ;
        Managers.Register.MboiTuiDefeated = instance.MboiTuiDefeated;
        Managers.Register.YasiYatereDefeated = instance.YasiYatereDefeated;
        Managers.Register.AoAoDefeated = instance.AoAoDefeated;

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