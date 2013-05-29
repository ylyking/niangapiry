using UnityEngine;
using System.Collections;

public class pickupThing : MonoBehaviour {

    public enum pickupType { custom = 0, maracuya, p_shot};
    public pickupType pickup = pickupType.custom;

    Transform thisTransform;

	void Start () 
    {
        thisTransform = transform;
        if (pickup == pickupType.maracuya)
        {
            rigidbody.Sleep();
            pickup = pickupType.custom;
        }
        enabled = false;
	}
	
	void Update () 
    {
        if ((pickup == 0) && gameObject.transform.parent == null)
            gameObject.tag = "pickup";
	
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

    void OnCollisionEnter(Collision hit)
    {
        if (thisTransform.parent == null && hit.transform.tag != "Player" && hit.transform.tag != "Enemy" )
            thisTransform.parent = Managers.Tiled.MapTransform ;
    }



}
