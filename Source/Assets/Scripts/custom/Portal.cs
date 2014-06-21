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
                Managers.Display.ShowFlash(1);
                Managers.Game.PopState();                                 // if it's an exit Return Map and stop inmediately
                return;
            }

            if (Target.ToLower().Contains(".tmx"))                        // if it had .tmx extension it's a map
            {
                Managers.Display.ShowFlash(1);

                if (Managers.Dialog.IsInConversation())
                    Managers.Dialog.DeInit();

                Managers.Tiled.Unload();

				if (Managers.Tiled.Load(Target.Remove(Target.LastIndexOf(".")) ) )
                {
                    //Managers.Register.SetPlayerPos();
                    Input.ResetInputAxes();
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
                        Input.ResetInputAxes();
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

            if ( Managers.Game.InputUp)
            {

                if (Target.ToLower() == "exit")
                {
                    Managers.Display.ShowFlash(1);
                    Managers.Game.PopState();                                 // if it's an exit Return Map
                    return;
                }

                if (Target.ToLower().Contains(".tmx"))
                {
                    Managers.Display.ShowFlash(1);
                    Managers.Tiled.Unload();

					if (Managers.Tiled.Load(Target.Remove(Target.LastIndexOf("."))) )
                    {
                        //Managers.Register.StartPoint = Vector3.zero;
                        //Managers.Register.SetPlayerPos();
                        Input.ResetInputAxes();
                        return;
                    }
                }

                foreach (Portal portal in GameObject.FindSceneObjectsOfType(typeof(Portal)))// else look in scene all Ids match
                {
                    if (portal.Id == Target)
                    {
                        Managers.Display.ShowFlash(1);
                        hit.transform.position = portal.gameObject.transform.position;
                        Input.ResetInputAxes();
                        return;

                    }
                }
            }
        }
    }


}
