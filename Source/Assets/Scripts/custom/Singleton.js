#pragma strict

public class MySingletonClass //: MonoBehaviour		
{
	private static var Instance : MySingletonClass = null;
    private function MySingletonClass() {;} 										// private Constructor protected
    public static function Get() : MySingletonClass 								// Singleton Instance Accessor 
    {
    	if(Instance == null)
    		Instance = new MySingletonClass();
        return Instance;
    }

    
    public function DoChachacha()
    { 
    	Debug.Log(" DoChachacha! "); 
    }
    

}

//function Start () 
//{
//	MySingletonClass.Get().DoChachacha();
//}

//function Update () {
//
//}