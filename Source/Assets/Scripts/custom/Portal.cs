using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour 
{

    public enum type{ warp = 0, door };  
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

    void OnTriggerStayEnter(Collider hit)
    {
        if (this.myType == type.warp && hit.tag == "Player")                           
        {
            //if (Target.ToLower().Contains(".tmx"))                                      // if it had .tmx extension it's a map
          
            //{
            //    Managers.Tiled.Unload();
            //    Managers.Tiled.Load(Target);
            //    return;
            //}

            foreach(Portal portal in GameObject.FindSceneObjectsOfType(typeof(Portal) ) )// else look in scene all Ids match
            {
                if ( portal.Id == Target ) 
                {
                    hit.transform.position = portal.gameObject.transform.position;
                    return;
                }
            }

            Managers.Game.PopState();                                                    // else Return Map
        }
    }

    void OnTriggerStay(Collider hit)
    {
        if (this.myType == type.door && hit.tag == "Player" && Input.GetAxis("Vertical") >0)
        {
            //if (Target.ToLower().Contains(".tmx"))
            //{
            //    Managers.Tiled.Unload();
            //    Managers.Tiled.Load(Target);
            //    return;
            //}

            foreach (Portal portal in GameObject.FindSceneObjectsOfType(typeof(Portal)))// else look in scene all Ids match
            {
                if (portal.Id == Target)
                {
                    hit.transform.position = portal.gameObject.transform.position;
                    return;
                }
            }

            Managers.Game.PopState();                                                    // else Return Map
        }
    }

}
