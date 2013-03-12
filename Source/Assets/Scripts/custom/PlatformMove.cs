
using UnityEngine;
using System.Collections;

public class PlatformMove : MonoBehaviour {

private  Vector3 StartPosition = Vector3.zero;
public  Vector3 EndPosition = new Vector3(2.0f, 0.0f, 0.0f);
public  float Speed = 0.5f;
private Transform thisTransform;

void  Start ()
{
	thisTransform = transform;
	StartPosition = transform.position;

    StartCoroutine( CoUpdate() );
}    

private IEnumerator CoUpdate()
{

    while (true) 
    {
    	var i=  Mathf.PingPong(Time.time * Speed, 1);
        thisTransform.position = Vector3.Lerp( StartPosition, StartPosition + EndPosition, i);
        yield return 0;
    }
}

}