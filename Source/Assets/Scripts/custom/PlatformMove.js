#pragma strict

private  var StartPosition 	: Vector3 = Vector3.zero;
public  var EndPosition 	: Vector3 = Vector3(2.0f, 0.0f, 0.0f);
public  var Speed		 	: float = 0.5f;
private var thisTransform 	: Transform;

function Start () 
{
	thisTransform = transform;
	StartPosition = transform.position;
	    
    while (true) 
    {
    	var i =  Mathf.PingPong(Time.time * Speed, 1);
        thisTransform.position = Vector3.Lerp( StartPosition, StartPosition + EndPosition, i);
        yield;
    }
}
