
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScrolling : MonoBehaviour {

public Transform target;	                            // The object in our scene that our camera is currently tracking.
private Transform thisTransform;	                    // .

private float distance	        = 1.0f;	                // How far back should camera be from the target?
private float springiness	    = 4.0f;	                // How strict should camera follow the target?  Lower input make the camera more lazy.
private float zoomFactor	    = 2.5f;	                // The orthogonal Camera field of view size, ( like a zoom for 2d).

public bool  ShowMapLimits      = true;

public float fallOutBuffer      = 5.0f;
public float colliderThickness  = 10.0f;
public float MinScreenLimit     = 0.0f;

//private Rect cameraBounds          = new Rect( 0, 0, 1, 3); 	// We set up these references in the Awake () function.
public Rect levelBounds            = new Rect( 0, 0, 100, 50); 	// We set up these references in the Awake () function.
private Rect originalBounds            = new Rect( 0, 0, 100, 50); 	// This it's for save old Boundaries.
private GameObject createdBoundaries;

void  OnDrawGizmos ()
{
        //Gizmos.color        = sceneViewDisplayColor;

        ////cameraBounds.center = transform.position + Vector3.down ;
        //Gizmos.DrawLine(new Vector2(cameraBounds.xMin, cameraBounds.yMin), new Vector2(cameraBounds.xMax, cameraBounds.yMin));
        //Gizmos.DrawLine(new Vector2(cameraBounds.xMin, cameraBounds.yMax), new Vector2(cameraBounds.xMax, cameraBounds.yMax) );
        //Gizmos.DrawLine(new Vector2(cameraBounds.xMin, cameraBounds.yMin), new Vector2(cameraBounds.xMin, cameraBounds.yMax) );
        //Gizmos.DrawLine(new Vector2(cameraBounds.xMax, cameraBounds.yMin), new Vector2(cameraBounds.xMax, cameraBounds.yMax));
    

     if (ShowMapLimits)
     {
	    Gizmos.color        = Color.blue;
	    Vector3 lowerLeft   = new Vector3 (levelBounds.xMin, levelBounds.yMax, 0);
	    Vector3 upperLeft   = new Vector3 (levelBounds.xMin, levelBounds.yMin, 0);
	    Vector3 lowerRight  = new Vector3 (levelBounds.xMax, levelBounds.yMax, 0);
	    Vector3 upperRight  = new Vector3 (levelBounds.xMax, levelBounds.yMin, 0);
    	
	    Gizmos.DrawLine (lowerLeft, upperLeft);
	    Gizmos.DrawLine (upperLeft, upperRight);
	    Gizmos.DrawLine (upperRight, lowerRight);
	    Gizmos.DrawLine (lowerRight, lowerLeft);
     }
}


private RigidbodyInterpolation savedInterpolationSetting = RigidbodyInterpolation.None;    

void  Awake ()
{
 	thisTransform = transform;
        //cameraBounds = new Rect( 0, 0, cameraBounds.width, cameraBounds.height); 
        //cameraBounds.center = thisTransform.position;
}


void LateUpdate()
{
    Vector3 goalPosition = GetGoalPosition ();	// Where should our camera be looking right now?

    // Set Zoom Level accord the target needs
    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, -zoomFactor, Time.deltaTime * springiness);
	
	// Interpolate between the current camera position and the goal position.
	thisTransform.position =   Vector3.Lerp ( thisTransform.position, goalPosition, Time.deltaTime * springiness) ;	
	
    //thisTransform.position.y = (float)System.Math.Round(thisTransform.position.y , 2);
}


Vector3 GetGoalPosition ()	// find out where the camera should move to, Based on camera and target's attributes
{

	if  (!target)	return thisTransform.position;			// If there isn't target, don't move the camera & return current position.
	
	// Our camera script can take attributes from the target.  
	// If there are no attributes attached, we have the following defaults.
	
	float heightOffset 	    = 0;		// How high in world space should the camera look above the target?
	float widthOffset		= 0;
	float distanceModifier 	= 2.5f;		// How much should we zoom the camera based on this target?
	float velocityLookAhead = 0;		// By default, we won't account for any target velocity in calculations;
	Vector2 maxLookAhead 	= new Vector2 (0, 0);
	
	// Look for CameraTargetAttributes in our target.
	CameraTargetAttributes cameraTargetAttributes = target.GetComponent<CameraTargetAttributes>();
	if  (cameraTargetAttributes) 		// If our target has special attributes, use these instead of our above defaults.
	{
		heightOffset 		= cameraTargetAttributes.Offset.y;
		widthOffset			= cameraTargetAttributes.Offset.x;
		distanceModifier 	= cameraTargetAttributes.distanceModifier;
		velocityLookAhead 	= cameraTargetAttributes.velocityLookAhead;
		maxLookAhead 		= cameraTargetAttributes.maxLookAhead;
	}
	
	// First do a rough goalPosition that simply follows the target at a certain relative height and distance.
	Vector3 goalPosition= target.position + new Vector3 ( widthOffset, heightOffset, -2.5f);
	zoomFactor = -distance * distanceModifier; 							// some provisory fixes for ortogonal camera
	
	
	// Next, we refine our goalPosition by taking into account our target's current velocity.
	// This will make the camera slightly look ahead to wherever the character is going:
	

    Vector3 targetVelocity = Vector3.zero;										// First assume there is no velocity.
    // in case camera's target isn't a Rigidbody, it won't do any look-ahead calculations because everything will be zero.
		
    var targetRigidbody= target.GetComponent<CharacterController>();		// If we find a Rigidbody on target.. 
    if (targetRigidbody)
	    targetVelocity = targetRigidbody.velocity;							// that means we can access at his velocity!
	
    // If you've had a physics class, you may recall an equation similar to: position = velocity * time;
    Vector3 lookAhead= targetVelocity * velocityLookAhead; 	// target's position will be in velocityLookAhead seconds.
	
    lookAhead.x = Mathf.Clamp(lookAhead.x, -maxLookAhead.x, maxLookAhead.x); // clamp the vector to some input 
    lookAhead.y = Mathf.Clamp(lookAhead.y, -maxLookAhead.y, maxLookAhead.y); // so the target doesn't go offscreen.
    lookAhead.z = 0;					// We never want to take z velocity into account as this is 2D.  Just keep it in zero.
	
	
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
	
	// Send back our spiffily calculated goalPosition back to the caller!
	return goalPosition;
}


public void SetTarget(Transform newTarget, bool snap = false)
{
    // If there was a target, reset its interpolation value if it had a rigidbody.
    if (target)
    {
        // Reset the old target's interpolation back to the saved value.
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody)
            targetRigidbody.interpolation = savedInterpolationSetting;
    }

    // Set our current target to be the value passed to SetTarget ()
    target = newTarget;

    // Now, save the new target's interpolation setting and set it to interpolate for now.
    // This will make our camera move more smoothly.  Only do this if we didn't set the
    // target to null (nothing).
    if (target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody)
        {
            savedInterpolationSetting = targetRigidbody.interpolation;
            targetRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    // If we should snap the camera to the target, do so now.
    // Otherwise, the camera's position will change in the LateUpdate () function.
    if (snap)
        transform.position = GetGoalPosition();
}

    
public void ResetBounds()
{
    ResetBounds( new Rect( 0, 0, 100, 50)) ;
}
    
public void ResetBounds( Rect Bounds )
{
    if ( createdBoundaries != null )
        Destroy(createdBoundaries);

    levelBounds = Bounds;
    originalBounds = levelBounds;

    createdBoundaries = new GameObject("Created Boundaries");
    //createdBoundaries.transform.parent = transform;

    GameObject leftBoundary = new GameObject("Left Boundary");
	leftBoundary.transform.parent = createdBoundaries.transform;
    BoxCollider boxCollider = (BoxCollider)leftBoundary.AddComponent(typeof(BoxCollider));
    boxCollider.size = new Vector3(colliderThickness, levelBounds.height + colliderThickness * 2.0f + fallOutBuffer, colliderThickness);
    boxCollider.center = new Vector3(levelBounds.xMin - colliderThickness * 0.5f, levelBounds.y + levelBounds.height * 0.5f - fallOutBuffer * 0.5f, 0.0f);

    GameObject rightBoundary = new GameObject("Right Boundary");
	rightBoundary.transform.parent = createdBoundaries.transform;
    boxCollider = (BoxCollider)rightBoundary.AddComponent(typeof(BoxCollider));
    boxCollider.size = new Vector3(colliderThickness, levelBounds.height + colliderThickness * 2.0f + fallOutBuffer, colliderThickness);
    boxCollider.center = new Vector3(levelBounds.xMax + colliderThickness * 0.5f, levelBounds.y + levelBounds.height * 0.5f - fallOutBuffer * 0.5f, 0.0f);

    GameObject topBoundary = new GameObject("Top Boundary");
	topBoundary.transform.parent = createdBoundaries.transform;
    boxCollider = (BoxCollider)topBoundary.AddComponent(typeof(BoxCollider));
	boxCollider.size = new Vector3 (levelBounds.width + colliderThickness * 2.0f, colliderThickness, colliderThickness);
    boxCollider.center = new Vector3(levelBounds.x + levelBounds.width * 0.5f, levelBounds.yMax + colliderThickness * 0.5f, 0.0f);

    GameObject  bottomBoundary = new GameObject("Bottom Boundary (Including Fallout Buffer)");
	bottomBoundary.transform.parent = createdBoundaries.transform;
    boxCollider = (BoxCollider)bottomBoundary.AddComponent(typeof(BoxCollider));
    boxCollider.size = new Vector3(levelBounds.width + colliderThickness * 2.0f, colliderThickness, colliderThickness);
    boxCollider.center = new Vector3(levelBounds.x + levelBounds.width * 0.5f, levelBounds.yMin - colliderThickness * 0.5f - fallOutBuffer, 0.0f);
	
	MinScreenLimit = gameObject.transform.position.y ;
}


public void ResetViewArea(Rect area)
{
    levelBounds = area; // this Changes the viewable area without resizing collision Bounds
}

public void ResetViewArea()
{
    levelBounds = originalBounds;
}
        

   
public Rect GetBounds()
    {
        return this.levelBounds;
    }

        //string DebugText = "";
    //void OnGUI()
    //{
    //    GUI.color = Color.black;
    //    if ( target)
    //    GUI.Label(new Rect((Screen.width * .25f), (Screen.height * .9f), 600, 200), DebugText);
    //}

}