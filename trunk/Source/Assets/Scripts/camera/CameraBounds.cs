using UnityEngine;
using System.Collections;

public class CameraBounds : MonoBehaviour 
{
    //static Rect LevelBounds;
    Rect AreaBounds;                                         // This it's a script for Adjust Viewable areas inside Level

    public Vector2 Offset = Vector2.zero;
    public float ZoomFactor = 2.5f;


	void Start () 
    {
        AreaBounds.width = transform.localScale.x;
        AreaBounds.height = transform.localScale.y;
        AreaBounds.center = transform.position;
        //LevelBounds = Managers.Display.cameraScroll.levelBounds;
	}

    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Player")
        {
            Managers.Display.cameraScroll.ResetViewArea(AreaBounds);
            hit.GetComponent<CameraTargetAttributes>().Offset = Offset;
            hit.GetComponent<CameraTargetAttributes>().distanceModifier = ZoomFactor;
        }
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "Player")
        {
            Managers.Display.cameraScroll.ResetViewArea();
            hit.GetComponent<CameraTargetAttributes>().Offset = Vector2.zero;
            hit.GetComponent<CameraTargetAttributes>().distanceModifier = 2.5f;
        }
    }
}
