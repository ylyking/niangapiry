using UnityEngine;
using System.Collections;

public class pickupThing : MonoBehaviour {

    public enum pickupType { custom = 0, p_shot};
    public pickupType pickup = pickupType.custom;

	void Start () 
    {
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



}
