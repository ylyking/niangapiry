using UnityEngine;
using System.Collections;

public class pickupThing : MonoBehaviour {

    public enum pickupType { custom = 0, maracuya = 1, shots = 2, p_shot = 3, pushable, mate};
    public pickupType pickup = pickupType.custom;

    Transform thisTransform;

	void Start () 
    {
        thisTransform = transform;

        switch (pickup)
        {
            case pickupType.maracuya:
                rigidbody.Sleep();
                //pickup = pickupType.custom;

                break;

            case pickupType.mate:
                if (thisTransform.rigidbody != null)
                    rigidbody.Sleep();

                if (Managers.Register.Treasure2)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
                break;
        }


        enabled = false;
	}
	
	void Update () 
    {
        if ((pickup == 0) && gameObject.transform.parent == null)
            gameObject.tag = "pickup";

        if ((int)pickup == 2 && thisTransform.parent == Managers.Game.PlayerPrefab.transform)
            thisTransform.localPosition = Vector3.up * .25f ;
	
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

    IEnumerator OnCollisionEnter(Collision hit)
    {
        if (thisTransform.parent == null && hit.transform.tag != "Player"
            && hit.transform.tag != "Platform" && hit.transform.tag != "Enemy")
            thisTransform.parent = Managers.Tiled.MapTransform;

        if (gameObject.CompareTag("p_shot") && !hit.transform.CompareTag("Item"))
        {
            yield return new WaitForSeconds(1);
            if ( gameObject != null)
                gameObject.tag = "pickup";
        }
  

    }

    void OnTriggerStay(Collider hit)
    {
        if (hit.CompareTag("Platform") && thisTransform.parent == null)
        {
            rigidbody.AddForce( new Vector3(Random.Range(-.25f,.25f), .2f, 0), ForceMode.VelocityChange);
        }
    }

    void OnCollisionStay(Collision hit)
    {
        if (hit.transform.CompareTag("Platform") && pickup != pickupType.pushable && thisTransform.parent == null)
        {
            rigidbody.AddForce(new Vector3(Random.Range(-.25f, .25f), .2f, 0), ForceMode.VelocityChange);
        }
    }



}
