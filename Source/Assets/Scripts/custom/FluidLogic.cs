using UnityEngine;
using System.Collections;

namespace CustomAnimations
{

    public class FluidLogic : MonoBehaviour
    {
        public GameObject SplashPrefab;
        public AudioClip[] Splash;


        void OnTriggerEnter(Collider hit)
        {
            Instantiate(SplashPrefab, hit.transform.position + Vector3.back + Vector3.down * .5f, Quaternion.Euler(270, 0, 0));
            Instantiate(SplashPrefab, hit.transform.position + Vector3.back  , transform.rotation);
            Managers.Audio.Play(Splash[(int)Random.Range(0, Splash.Length - 1)], hit.transform);

            if (hit.tag == "Player"
                //&& Input.GetAxis("Horizontal") != 0  && Input.GetButton("Jump") )
                && (Input.GetAxis("Vertical") > -0.95f && Input.GetAxis("Vertical") < 0) )
            {
                ((PlayerControls)hit.GetComponent<PlayerControls>()).velocity.y = 7;
            }
        }

        void OnTriggerExit(Collider hit)
        {
            Instantiate(SplashPrefab, hit.transform.position + Vector3.back + Vector3.down * .5f, Quaternion.Euler(270, 0, 0));
            Instantiate(SplashPrefab, hit.transform.position + Vector3.back, transform.rotation);
        
        }
    }

}
