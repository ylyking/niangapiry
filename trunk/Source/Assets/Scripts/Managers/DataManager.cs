using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

public class DataManager : MonoBehaviour 
{

/// <summary>
/// COMMON GAMEPLAY DATA FIELDS
/// Here We have dfault ingame properties
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


/// <summary>
/// WORLD 0: HOME
/// </summary>




/// <summary>
/// WORLD 1: Monte
/// </summary>





/// <summary>
/// WORLD 2: Pampero
/// </summary>





/// <summary>
/// WORLD 3: Iguazú
/// </summary>





/// <summary>
/// WORLD 4: Campo del Cielo
/// </summary>






/// <summary>
/// WORLD 5: Impenetrable
/// </summary>
    
    
    // Use this for initialization
	void Start ()
    {
	
	}

    void Reset()
    {
        Score = 0;
        Fruits = 0;
        TopScore = 100;

        FireGauge = 0;
        Key = 0;
        Health = 3;
        Lifes = 3;
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