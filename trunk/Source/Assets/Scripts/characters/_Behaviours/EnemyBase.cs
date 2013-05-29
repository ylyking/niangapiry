using UnityEngine;
using System.Collections;


    public class EntityAI : MonoBehaviour
    {

        protected Behaviours.BehaviourBase ActiveBehaviour = null;                       // Behaviour Composition over Enemy

        void Start()
        {
            Behaviours.BehaviourBase state = new Behaviours.BehaviourSleep();

            if (state == null)
                Debug.Log("Smells like shit");
            SetBehaviour(state);

        }

        public Behaviours.BehaviourBase GetBehaviour()
        {
            return ActiveBehaviour;
        }
        
        public void SetBehaviour( Behaviours.BehaviourBase behaviour)
        {
            if (ActiveBehaviour != null)
            {
                ActiveBehaviour.OnDeinit();
                ActiveBehaviour = null;
            }
            
            ActiveBehaviour = behaviour;

            if (ActiveBehaviour != null)
            {
                ActiveBehaviour.OnInit( gameObject);
                //ActiveBehaviour->SetTarget(posX, posY, posZ);
            }
            else
                Debug.Log("Nothing created");
        }

        public void Update()
        {
            if (ActiveBehaviour != null) ActiveBehaviour.OnUpdate();
        }

        void OnDestroy()
        {
	        if (ActiveBehaviour != null)
	        {
		        ActiveBehaviour.OnDeinit() ;
                ActiveBehaviour = null;
		        //this is it
	        }
        }

    }

