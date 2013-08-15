using UnityEngine;
using System.Collections;

namespace BonusObjects
{
    public class FishBass : MonoBehaviour
    {
        public AudioClip Fisher;

        void OnTriggerStay(Collider other)
        {

            if (other.CompareTag("Player")

                && (Managers.Game.InputDown || Managers.Game.InputUp || Input.GetButton("Fire1"))
                && Managers.Game.State != Managers.Game.GetComponentInChildren<FishingState>())
                Managers.Game.PushState(typeof(FishingState));
        }
    }

}