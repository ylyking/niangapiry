#pragma strict

// Instantiates a projectile every 0.1 seconds,
// if the Fire1 button is pressed or held down.


public var projectile 	: GameObject;

public var target		: Transform;

public var damping 		: float = 4.0;
public var BulletOffset : float = 2.0;
public var fireRate		: float	 = 1.0;
public var Velocity 	: float = 25;

private var nextFire	: float = 0.0;

function Update ()
{
	var lookPos : Quaternion = Quaternion.LookRotation(transform.position - target.position, Vector3.forward);
	
	lookPos.y = 0;
	lookPos.x = 0;

	transform.rotation = Quaternion.Slerp(transform.rotation, lookPos, Time.deltaTime * damping);

    //If the player holds down or presses the left mouse button
    if (Input.GetKey ("x") && Time.time > nextFire)
    {
        Shoot();
    }

}

//To make our scripting a little more Object-Oriented-Programming, we will create our custom functions as well
function Shoot() 
{

    //Add fireRate and current time to nextFire
    nextFire = Time.time + fireRate;

	//Instantiate the projectile
    var clone  = Instantiate (projectile, transform.position , transform.rotation);
    
    clone.transform.Translate( Vector3( 0, BulletOffset, 0) ); // avoid hits between shot & shooter own colliders  
//    clone.transform.Translate( Vector3( 0, transform.localScale.y * 0.6, 0) ); // avoid hits between shot & shooter own colliders  

    //Name the clone "Shot" ::: this name will appear in the Hierarchy View when you Instantiate the object
    clone.name = "Shot";

	//Add speed to the target
    clone.rigidbody.velocity = transform.TransformDirection (Vector3.up * Velocity);
//    clone.rigidbody.AddForce(  Vector3.up  * Velocity, ForceMode.Impulse);
	
}