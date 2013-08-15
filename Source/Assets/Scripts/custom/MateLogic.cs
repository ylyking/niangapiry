using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace BonusObjects
{

    public class MateLogic : MonoBehaviour
    {
        Transform thisTransform;

        int totalMix = 3;

        int totalCheers = 3;
        float timeLapse = 0;

        public GameObject Treasure2Prefab;
        Transform TreasureTransform;
        Transform PavaTransform = null;

        public GameObject StarsParticle;
        public GameObject CheersParticle;

        enum MateStates { Normal, Congratulations, Disabled };
        MateStates currentState = MateStates.Normal;

        void Start()
        {
            thisTransform = transform;
            if (Managers.Register.Treasure2)
            {
                GameObject naming = (GameObject)Instantiate(Treasure2Prefab,
                                 thisTransform.position + Vector3.back * .1f + Vector3.up * 2,
                                 thisTransform.rotation);
                naming.name = "Treasure2";
                return;
            }

            StartCoroutine(CoUpdate());
        }

        IEnumerator CoUpdate()
        {
            while (true)
            {
                switch (currentState)
                {

                    case MateStates.Normal:
                        if (!Managers.Register.Treasure2)
                        {
                            if ( PavaTransform == null && GameObject.Find("matepava") )
                            {
                                GameObject Dilma = GameObject.Find("matepava") ;
                                PavaTransform = Dilma.transform;
                            }

                            if (PavaTransform != null)
                            {
                                if (PavaTransform.parent == Managers.Game.PlayerPrefab.transform)
                                    PavaTransform.localPosition = Vector3.up ;

                                if (PavaTransform.position.x > 142
                                && PavaTransform.position.x < 144 && PavaTransform.position.y > 12)
                                {
                                    if (PavaTransform.parent == Managers.Game.PlayerPrefab.transform)
                                    {
                                        PavaTransform.parent = null;        		//resets the pickup's parent to null so it won't keep following the player	
                                        PavaTransform.collider.enabled = true;
                                        PavaTransform.rigidbody.isKinematic = false;

                                        Managers.Game.PlayerPrefab.GetComponent<PlayerProperties>()._pickedObject = null;
                                    }
                                    PavaTransform.position = new Vector3(144, PavaTransform.position.y, PavaTransform.position.z);
                                    PavaTransform.rigidbody.AddForce(new Vector3(1, 0, 0) * 6, ForceMode.VelocityChange);
                                }
                            }

 
                            if (totalMix == 0)
                            {
                                Managers.Register.Treasure2 = true;
                                currentState = MateStates.Congratulations;
                            }
                        }
                        break;

                    case MateStates.Congratulations:

                        if (timeLapse <= 0)
                        {
                            timeLapse = 3;
                            totalCheers--;
                            Instantiate(CheersParticle, thisTransform.position + Vector3.up * 2, thisTransform.rotation);
                            Instantiate(CheersParticle, thisTransform.position + Vector3.up + Vector3.right, thisTransform.rotation);
                            Instantiate(CheersParticle, thisTransform.position + Vector3.up + Vector3.left, thisTransform.rotation);
                        }

                        timeLapse -= Time.deltaTime;

                        if (totalCheers <= 0)
                        {
                            TreasureTransform = ((GameObject)Instantiate(Treasure2Prefab,
                                 thisTransform.position + Vector3.back * .1f + Vector3.up * 2,
                                 thisTransform.rotation)).transform;
                            TreasureTransform.name = "Treasure2";
                            currentState = MateStates.Disabled;
                        }

                        break;

                    case MateStates.Disabled:
                        if (TreasureTransform != null && TreasureTransform.position.y > thisTransform.position.y)
                            TreasureTransform.position += Vector3.down * Time.deltaTime;

                        break;
                }

                yield return 0;
            }
        }

        IEnumerator OnTriggerEnter(Collider hit)
        {
            if ( (hit.tag == "pickup" || hit.tag == "p_shot") &&
                (hit.name == "matebombilla" || hit.name == "mateyerba" || hit.name == "matepava"))
            {
                //Debug.Log("Getting Shit");
                yield return new WaitForSeconds(0.5f);
                totalMix--;
                Instantiate(StarsParticle, hit.transform.position, thisTransform.rotation);
                Destroy(hit.gameObject);
                yield return 0;
            }

        }

        //void OnCollisionStay(Collision hit)
        //{
        //    if ((hit.gameObject.tag == "pickup" || hit.gameObject.tag == "p_shot") &&
        //        (hit.gameObject.name == "matebombilla" || hit.gameObject.name == "mateyerba" || hit.gameObject.name == "matepava"))
        //    {
        //        Debug.Log("Getting Shit");
        //        totalMix--;
        //        Instantiate(StarsParticle, hit.transform.position, thisTransform.rotation);
        //        Destroy(hit.gameObject);
        //    }

        //}

        //void OnTriggerEnterStay(Collider hit)
        //{
        //    if ((hit.tag == "pickup" || hit.tag == "p_shot") &&
        //        (hit.name == "MateBombilla" || hit.name == "MateYerba" || hit.name == "MatePava"))
        //    {
        //        Debug.Log("Getting Shit");
        //        totalMix--;
        //        Instantiate(StarsParticle, hit.transform.position, thisTransform.rotation);
        //        Destroy(hit.gameObject);
        //    }

        //}



    }

}
