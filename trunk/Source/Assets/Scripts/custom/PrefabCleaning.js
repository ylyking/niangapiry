#pragma strict
// a little script for auto-destroy projectile instances when get out of screen or after some time lapse...

public var	ScreenOffset 	: float = 50;

public	var LifeTime		: float = 5;
private	var TotalTime 		: float ;

function Start ()
{
	TotalTime = Time.time + LifeTime;		
}

function Update ()
 {
//	print("bullet distance " + Mathf.Abs( transform.position.y));
 
//	if ( Mathf.Abs( transform.position.y) > ScreenOffset || Mathf.Abs( transform.position.x) > ScreenOffset )

	if ( Mathf.Abs( transform.position.y) > ScreenOffset || Time.time > TotalTime )
		Destroy(gameObject);
}