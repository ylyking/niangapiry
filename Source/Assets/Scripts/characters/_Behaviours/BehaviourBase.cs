using UnityEngine;
using System.Collections;

namespace Behaviours                            // Some Enemies Behviours
{
    public class BehaviourBase  
    {
        public  GameObject Character;

        public virtual void OnInit(GameObject enemy)
        {
            Character = enemy;
            Debug.LogWarning("Default Base Input: Not Processing nothing ");
        }

        public virtual void OnDeinit() { Character = null; }


        public virtual void OnUpdate()
        {
        }
    }


}
