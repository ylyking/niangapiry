using UnityEngine;
using System.Collections;


// This script goes on any GameObject in your scene that you will track with the camera.
// It'll help customize the camera tracking to your specific object to polish your game.
public class CameraTargetAttributes : MonoBehaviour {


// See the GetGoalPosition () function in CameraScrolling.js for an explanation of these variables.
public bool FixedHeight				= false;
public Vector3 Offset               = Vector2.zero;
public float distanceModifier 		= 2;
public float velocityLookAhead 		= 0.15f;
public Vector2 maxLookAhead 		= new Vector2 (3, 3);
}