
using UnityEngine;
using System.Collections;

public class CameraScrolling : MonoBehaviour {

public Transform target;	// The object in our scene that our camera is currently tracking.
private Transform thisTransform;	// The object in our scene that our camera is currently tracking.


private float distance	= 1.0f;	// How far back should camera be from the target?
private float springiness	= 4.0f;	// How strict should camera follow the target?  Lower input make the camera more lazy.

//private LevelAttributes levelAttributes;			// Keep handy reference store our level's attributes. 
private Rect levelBounds; 						// We set up these references in the Awake () function.
//private bool targetLock= false;


// This is for setting interpolation on our target, but making sure we don't permanently
// alter the target's interpolation setting.  This is used in the SetTarget () function.
//private RigidbodyInterpolation savedInterpolationSetting = RigidbodyInterpolation.None;   /// <<<<<<<<<<<<<<<<<< VINOS HEREFORD

void  Awake (){
 	thisTransform = transform;
    if (Managers.Stages.GetComponent<LevelAttributes>())
        levelBounds = Managers.Stages.GetComponent<LevelAttributes>().bounds;
	else 														// else use some defaults input..
	{ levelBounds.xMin = levelBounds.yMin = 0; levelBounds.xMax = levelBounds.yMax = 10; }

}

public void ResetBounds()
{
    var levelAttribs =    (Managers.Stages.GetComponent<LevelAttributes>());
    if (levelAttribs != null && this.levelBounds != levelAttribs.bounds)
        levelBounds = levelAttribs.bounds;
}


void Update()
{
	Vector3 goalPosition = GetGoalPosition ();	// Where should our camera be looking right now?

    // Set Zoom Level accord the target needs
    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, -goalPosition.z, Time.deltaTime * springiness);

    // I need to refactor all this thing, It sucks hard!
    //goalPosition.z = -50;
	
	// Interpolate between the current camera position and the goal position.
	thisTransform.position =   Vector3.Lerp ( thisTransform.position, goalPosition, Time.deltaTime * springiness) ;	
	
    //thisTransform.position.y = (float)System.Math.Round(thisTransform.position.y , 2);
	
	 									 
// You almost always want camera motion to go inside of LateUpdate (), so that the camera follows
// the target after_ it has moved.  Otherwise, the camera may lag one frame behind.
}

Vector3 GetGoalPosition ()	// find out where the camera should move to, Based on camera and target's attributes
{

	if  (!target)	return thisTransform.position;			// If there isn't target, don't move the camera & return current position.
	
	// Our camera script can take attributes from the target.  
	// If there are no attributes attached, we have the following defaults.
	
	bool  FixedHeight 	    = false;	// this it's used for set fixed the camera's vertical move 
	float heightOffset 	    = 0.0f;		// How high in world space should the camera look above the target?
	float widthOffset		= 0.0f;
	float distanceModifier 	= 2.0f;		// How much should we zoom the camera based on this target?
	float velocityLookAhead = 0.0f;		// By default, we won't account for any target velocity in calculations;
	Vector2 maxLookAhead 	= new Vector2 (0.0f, 0.0f);
	
	// Look for CameraTargetAttributes in our target.
	CameraTargetAttributes cameraTargetAttributes = target.GetComponent<CameraTargetAttributes>();
	if  (cameraTargetAttributes) 		// If our target has special attributes, use these instead of our above defaults.
	{
		FixedHeight 		= cameraTargetAttributes.FixedHeight;
		heightOffset 		= cameraTargetAttributes.Offset.y;
		widthOffset			= cameraTargetAttributes.Offset.x;
		distanceModifier 	= cameraTargetAttributes.distanceModifier;
		velocityLookAhead 	= cameraTargetAttributes.velocityLookAhead;
		maxLookAhead 		= cameraTargetAttributes.maxLookAhead;
	}
	
	// First do a rough goalPosition that simply follows the target at a certain relative height and distance.
	//	var goalPosition= target.position + Vector3 (0, heightOffset,  -distance * distanceModifier );
	Vector3 goalPosition= target.position + new Vector3 ( widthOffset, heightOffset, 0);
	goalPosition.z = -distance * distanceModifier; 							// some provisory fixes for ortogonal camera
	
	
	// Next, we refine our goalPosition by taking into account our target's current velocity.
	// This will make the camera slightly look ahead to wherever the character is going:
	
	Vector3 targetVelocity = Vector3.zero;										// First assume there is no velocity.
	// in case camera's target isn't a Rigidbody, it won't do any look-ahead calculations because everything will be zero.
	
	
	var targetRigidbody= target.GetComponent<CharacterController>();		// If we find a Rigidbody on target.. 
	if (targetRigidbody)
		targetVelocity = targetRigidbody.velocity;							// that means we can access at his velocity!
	
//	// If we find a PlatformerController on the target, we can access a velocity from that!
//	targetPlatformerController = target.GetComponent (PlatformerController);
//	if (targetPlatformerController)
//		targetVelocity = targetPlatformerController.GetVelocity ();
	
	// If you've had a physics class, you may recall an equation similar to: position = velocity * time;
	Vector3 lookAhead= targetVelocity * velocityLookAhead; 	// target's position will be in velocityLookAhead seconds.
	
	lookAhead.x = Mathf.Clamp(lookAhead.x, -maxLookAhead.x, maxLookAhead.x); // clamp the vector to some input 
	lookAhead.y = Mathf.Clamp(lookAhead.y, -maxLookAhead.y, maxLookAhead.y); // so the target doesn't go offscreen.
	lookAhead.z = 0.0f;					// We never want to take z velocity into account as this is 2D.  Just keep it in zero.
	
	
	goalPosition += lookAhead;			// Now add in our lookAhead calculation.  Our camera following is now a bit better!
	
	// To put the icing on the cake, we will make so the positions beyond the level boundaries are never seen.  
	Vector3 clampOffset= Vector3.zero;		// This gives your level a great contained feeling, with a definite beginning and end.
	
	Vector3 cameraPositionSave= thisTransform.position;							 // But first, save the previous position.
	thisTransform.position = goalPosition;	// Temporarily set camera to the goal position so we can test positions for clamping.
	
	// Get the target position in viewport space.  Viewport space is relative to the camera.
	// bottom left is (0,0) and upper right is (1,1)
	Vector3 targetViewportPosition= camera.WorldToViewportPoint (target.position);
	
	// 1º clamp to the right & top. After this we will clamp to the bottom & left, so it will override this clamping
	// if it needs to. only in case that level is too smaller that camera sees more than the entire level at once.
	
	// What is the world position of the very upper right corner of the camera?
	Vector3 upperRightCameraInWorld= camera.ViewportToWorldPoint (new Vector3 (1.0f, 1.0f, targetViewportPosition.z));
	
	// Find out how far outside the world the camera is right now.
	clampOffset.x = Mathf.Min (levelBounds.xMax - upperRightCameraInWorld.x, 0.0f);
	clampOffset.y = Mathf.Min ((levelBounds.yMax - upperRightCameraInWorld.y), 0.0f);
	
	// Now we apply our clamping to our goalPosition.  Now our camera won't go past the right and top boundaries of the level!
	goalPosition += clampOffset;
	
	// Now we do basically the same thing, except clamp to the lower left of the level.  This will override any previous clamping
	// if the level is really small.  That way you'll for sure never see past the lower-left of the level, but if the camera is
	// zoomed out too far for the level size, you will see past the right or top of the level.
	
	thisTransform.position = goalPosition;
	Vector3 lowerLeftCameraInWorld= camera.ViewportToWorldPoint (new Vector3 (0.0f, 0.0f, targetViewportPosition.z));
	
	// Find out how far outside the world the camera is right now.
	clampOffset.x = Mathf.Max ((levelBounds.xMin - lowerLeftCameraInWorld.x), 0.0f);
	clampOffset.y = Mathf.Max ((levelBounds.yMin - lowerLeftCameraInWorld.y), 0.0f);
	
	// Now we apply our clamping to our goalPosition once again.  Now our camera won't go past the left and bottom boundaries of the level!
	goalPosition += clampOffset;
	
	// Now that we're done calling functions on the camera, we can set the position back to the saved position;
	thisTransform.position = cameraPositionSave;
	
	if (FixedHeight)						// in case we set fixed the height position set the global value;
		goalPosition.y = heightOffset;
	
	// Send back our spiffily calculated goalPosition back to the caller!
	return goalPosition;
}

//
//// Provide another version of SetTarget that doesn't require the snap variable to set.
//// This is for convenience and cleanliness.  By default, we will not snap to the target.
//void  SetTarget ( Transform newTarget  ){
//	SetTarget (newTarget, false);
//}
//
//// This is a simple accessor function, sometimes called a "getter".  It is a publically callable
//// function that returns a private variable.  Notice how target defined at the top of the script
//// is marked "private"?  We can not access it from other scripts directly.  Therefore, we just
//// have a function that returns it.  Sneaky!
//void  GetTarget (){
//	return target;
//}
//
//
//void  SetTarget ( Transform newTarget ,   bool snap  ){
 //	// If there was a target, reset its interpolation value if it had a rigidbody.
//	if  (target) {
//		// Reset the old target's interpolation back to the saved value.
//		Rigidbody targetRigidbody = target.GetComponent (Rigidbody);
//		if  (targetRigidbody)
//			targetRigidbody.interpolation = savedInterpolationSetting;
//	}
//	
//	// Set our current target to be the value passed to SetTarget ()
//	target = newTarget;
//	
//	// Now, save the new target's interpolation setting and set it to interpolate for now.
//	// This will make our camera move more smoothly.  Only do this if we didn't set the
//	// target to null (nothing).
//	if (target) {
//		targetRigidbody = target.GetComponent (Rigidbody);
//		if (targetRigidbody) {
//			savedInterpolationSetting = targetRigidbody.interpolation;
//			targetRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
//		}
//	}
//	
//	// If we should snap the camera to the target, do so now.
//	// Otherwise, the camera's position will change in the LateUpdate () function.
//	if  (snap) {
//		transform.position = GetGoalPosition ();
//	}
//}

}