using UnityEngine;
using System.Collections;

namespace Behaviours                            // Some Enemies Behviours
{
    public class BehaviourBase  
    {
        public GameObject Character;
        public Transform thisTransform;
        public EntityAI thisData;

        public virtual void OnInit(GameObject enemy)
        {
            Character = enemy;
            thisTransform = Character.transform;
            thisData = Character.GetComponent<EntityAI>();

            //Debug.LogWarning("Default Base Input: Not Processing nothing ");
        }

        public virtual void OnDeinit() { Character = null; }


        public virtual void OnUpdate()
        {
        }

        public virtual void OnTriggerEnter(Collider hit)
        {
            Debug.Log("Calling base trigger enter");
        }

        public virtual void OnTriggerExit(Collider hit)
        {
            Debug.Log("Calling base trigger exit");
        }

        public virtual void OnTriggerStay(Collider hit)
        {
            Debug.Log("Calling base trigger stay");
        }
    }


}
