#pragma strict

//private var orientation ;					// Move Direction: -1 == left, +1 == right

//public var HoldPosition 		: Transform ; 		//This is the point where the "hands" of the player would be
//public var GrabHeight   		: float 	= 0.7f;				
public var GrabPosition   		: Vector3 	= Vector3( 0f, 0.5f, 0f);				
public var ThrowForce   		: float		= 400f;	//How strong the throw is. This assumes the picked object has a rigidbody component attached
public var AlphaAmount  		: float 	= 0.5f; //this will be the alpha amount. It's public so you can change it in the editor

private var _pickedObject 		: Transform;
private var _originalColor 		: Color;
private var HoldingObj			: boolean 	= false;


function Start ()
{
//	HoldPosition = this.transform;
		
}

function Update ()
{

	if (Input.GetButton("Fire1"))
    {
    	if (_pickedObject && !HoldingObj )
         {
//		  HoldingObj = true;
	      var orientation = GetComponent(PlayerControls).orientation;
		
          //resets the pickup's parent to null so it won't keep following the player
          _pickedObject.parent = null;

          //resets the pickup's color so it won't stay half transparent forever
          _pickedObject.renderer.material.color = _originalColor;
          
          _pickedObject.rigidbody.isKinematic = false;
          
          _pickedObject.collider.enabled = true;
                    
//          _pickedObject.collider.isTrigger = true;
                    
          //applies force to the rigidbody to create a throw
//          _pickedObject.rigidbody.AddForce( Vector3( orientation, 1,0) * ThrowForce, ForceMode.Impulse);
          _pickedObject.rigidbody.AddForce( Vector3( orientation, Input.GetAxis( "Vertical"),0) * ThrowForce, ForceMode.Impulse);
          
//          _pickedObject.rigidbody.useGravity = true;
  
          //resets the _pickedObject 
          _pickedObject = null;
         }
    }
    if (Input.GetButtonUp("Fire1")) HoldingObj = false;

}


function OnControllerColliderHit (hit : ControllerColliderHit)
{
       if ( !_pickedObject && hit.transform.tag == "pickup" && Input.GetButtonDown( "Fire1") )
       {
         HoldingObj = true;
             	
         //caches the picked object
         _pickedObject = hit.transform;
         
         ThrowForce = _pickedObject.rigidbody.mass * 5;
         
         _pickedObject.rigidbody.isKinematic = true;
         
         _pickedObject.collider.enabled = false;
                  
         //caches the picked object's color for resetting later
//         _originalColor = hit.transform.renderer.material.color;
         _originalColor = _pickedObject.renderer.material.color;

         //this will snap the picked object to the "hands" of the player
         _pickedObject.position = transform.position + GrabPosition; 
//		_pickedObject.position = HoldPosition.position ;

         //this will set the HoldPosition as the parent of the pickup so it will stay there
         _pickedObject.parent = this.transform; 
//         _pickedObject.parent = HoldPosition; // = this.transform;

         //this will change the alpha amount on the object's color to make it half transparent
         _pickedObject.renderer.material.color = new Color( _pickedObject.renderer.material.color.r,
                             								_pickedObject.renderer.material.color.g,
                            								_pickedObject.renderer.material.color.b,
                                        					 AlphaAmount);
                                        					 
       }
       
}

//function PlayerThrows()
//{
//    	if ( _pickedObject )
//         {
//	      var orientation = playerControls.orientation;
//		
//          _pickedObject.parent = null;        		//resets the pickup's parent to null so it won't keep following the player
//
//          _pickedObject.renderer.material.color = _originalColor; 	//resets pickup's color so it won't stay half transparent 
//          
//          _pickedObject.rigidbody.isKinematic = false;				// check this if you re using physics Unity system
//          
//          _pickedObject.collider.enabled = true;
//                    
//          _pickedObject.collider.isTrigger = true;
//                    
//          //applies force to the rigidbody to create a throw
////          _pickedObject.rigidbody.AddForce( Vector3( orientation, 1,0) * ThrowForce, ForceMode.Impulse);
//          _pickedObject.rigidbody.AddForce( Vector3( orientation, Input.GetAxis( "Vertical"),0) * ThrowForce, ForceMode.Impulse);
//          
//          _pickedObject.rigidbody.useGravity = true;				// check this if you re using physics Unity system
//  
//          //resets the _pickedObject 
//          _pickedObject = null;
//         }
// }


// additional function to push rigidbodies with th character controller

//function OnControllerColliderHit (hit : ControllerColliderHit) 
//{	
//
////	Debug.Log("ControllerCollidig something: " + hit.gameObject.name + " with player!" );
//
//
//	if( hit.gameObject.name == "Gaucho" && controller.collisionFlags & CollisionFlags.Sides )
//	{ 
////		Debug.Log(" Enemy: " + controller.collider.transform.position.y );
////		Debug.Log(" Player: " + hit.collider.transform.position.y);
////		
////		if( controller.transform.position.y <= hit.transform.position.y	)
////				print("Enemy hitting side: " + hit.gameObject.name);
//				
//		if( controller.collider.transform.position.y <= hit.collider.transform.position.y	)
//				print("Enemy hitting side: " + hit.gameObject.name);
//	
//	}
//
//
//	// this script pushes all rigidbodies that the character touches
//	var pushPower : float = 1.50;
//
//
//    var body : Rigidbody = hit.collider.attachedRigidbody;
//    
//    // no rigidbody
//    if ( !body || body.isKinematic ) { return; }
// 
//         // We dont want to push objects below us
//    if (hit.moveDirection.y < -0.3) { return; }
//
//    // Calculate push direction from move direction,
//    // we only push objects to the sides never up and down
//    var pushDir = Vector3 (hit.moveDirection.x, 0, 0);
//    
////    var pushDir = Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);
//    
//
//    // If you know how fast your character is trying to move,
//    // then you can also multiply the push velocity by that.
//
//    // Apply the push
////    body.velocity = pushDir * pushPower;
////	if ( body.tag == "pickup" )//	controller.s 0.0001
//		 body.AddForce(pushDir * pushPower, ForceMode.Impulse);
//}