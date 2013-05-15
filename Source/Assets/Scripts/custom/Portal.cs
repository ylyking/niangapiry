using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour 
{

    public enum type{ warp = 0, door, start };  
    public type myType = type.warp;

    public string Target;
    public string Id = "";
    //public Vector3 Target = Vector3.zero;

    public void SetType(type tipo)
    {
        myType = tipo;
    }

    public void SetId(string id)
    {
        Id = id;
    }

    public void SetTarget(string target)
    {
        Target = target;
    }

    void OnTriggerEnter(Collider hit)
    {
        if ( this.myType == type.warp && hit.tag == "Player")										      // check tag savepoint
        {


            if (Target.ToLower() == "exit")
            {
                Managers.Game.PopState();                                 // if it's an exit Return Map and stop inmediately
                return;
            }

            if (Target.ToLower().Contains(".tmx"))                        // if it had .tmx extension it's a map
            {
                if (Managers.Dialog.IsInConversation())
                    Managers.Dialog.DeInit();

                Managers.Display.ShowFlash(0.5f);
                Managers.Tiled.Unload();

                if (Managers.Tiled.Load(Target))
                {
                    Managers.Register.SetPlayerPos();
                    return;
                }
            }

            if (Target != "")
                foreach(Portal portal in GameObject.FindSceneObjectsOfType(typeof(Portal) ) )// else look in scene all Ids match
                {
                    if ( portal.Id == Target ) 
                    {
                        Managers.Display.ShowFlash(0.5f);
                        hit.transform.position = portal.gameObject.transform.position;
                        return;
                    }
                }

            //  else current player position is saved as a checkpoint
            Managers.Register.MapCheckPoints[Managers.Register.currentLevelFile] = transform.position; 
        }
    }

    void OnTriggerStay(Collider hit)
    {
        //if (this.myType == type.door && hit.tag == "Player" && Input.GetAxis("Vertical") >0)
        if (this.myType == type.door && hit.tag == "Player")
        {
                                                                            // current player position is saved      
            Managers.Register.MapCheckPoints[Managers.Register.currentLevelFile] = transform.position; 

            if (InputUp)
            {
                if (Target.ToLower() == "exit")
                {
                    Managers.Game.PopState();                                 // if it's an exit Return Map
                    return;
                }

                if (Target.ToLower().Contains(".tmx"))
                {
                    Managers.Display.ShowFlash(0.5f);
                    Managers.Tiled.Unload();

                    if (Managers.Tiled.Load(Target))
                    {
                        //Managers.Register.StartPoint = Vector3.zero;
                        Managers.Register.SetPlayerPos();
                        return;
                    }
                }

                foreach (Portal portal in GameObject.FindSceneObjectsOfType(typeof(Portal)))// else look in scene all Ids match
                {
                    if (portal.Id == Target)
                    {
                        Managers.Display.ShowFlash(0.5f);
                        hit.transform.position = portal.gameObject.transform.position;
                        return;
                    }
                }
            }
        }
    }

    static bool ToggleUp = true;
    static bool InputUp                             // This it's a little oneShot Up Axis check for doors & like   
    {
        get
        {
            if (Input.GetAxis("Vertical") == 0)                      // It's like an "Input.GetAxisDown" 
                ToggleUp = true;

            if (ToggleUp == true && Input.GetAxis("Vertical") > 0)
            {
                ToggleUp = false;
                return true;
            }
            return false;
        }
    }

}
