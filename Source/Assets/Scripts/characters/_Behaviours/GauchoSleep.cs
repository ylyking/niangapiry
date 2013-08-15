using UnityEngine;
using System.Collections;

namespace Behaviours                            // Some Enemies Behviours
{
    public class GauchoSleep : Behaviours.BehaviourBase
    {
        public float alertRange = 2;			// set the range for finding the player
        float AlertRangeX2 = 0;
        float distanceToTarget = 0;			// the dist to target position stored in square mag.
   
        float TimeLapse = 0;
        float GroundPosition = 0;


        public override void OnInit(GameObject enemy)
        {
            base.OnInit(enemy);
            AlertRangeX2 = alertRange * alertRange;
        }

        //public override void OnDeinit()    { ;}


        public override void OnUpdate()
        {
            if (!thisData.target)
                if (Managers.Game.PlayerPrefab)
                {
                    thisData.target = Managers.Game.PlayerPrefab.transform;
                    thisData.linkToPlayerControls = (PlayerControls)thisData.target.GetComponent<PlayerControls>();
                }
                else return;

            distanceToTarget = (thisData.target.position - thisTransform.position).sqrMagnitude;	// it's much faster using Square mag. 

            if (distanceToTarget <= AlertRangeX2)
            {
                if (TimeLapse == 0)
                {
                    TimeLapse = Time.time + 0.55f;
                    GroundPosition = thisTransform.position.y;
                }

                if (TimeLapse > Time.time)
                {
                    thisTransform.position = new Vector3(thisTransform.position.x,
                                                         GroundPosition + Mathf.Sin((TimeLapse - Time.time) * 5) * .5f,
                                                         thisTransform.position.z);
                    thisData.animPlay.PlayFramesFixed(1, 0, 2, thisData.orientation); 				// Do Wake up Animation

                    return;
                }
                else
                    thisData.SetBehaviour(new Behaviours.GauchoSearch());
            }
            else
            {
                thisData.velocity = Vector3.zero;
                thisData.animPlay.PlayFrames(0, 0, 4, thisData.orientation, 2);							// Do zzZZZ Animation
            }
        }
    }


}
