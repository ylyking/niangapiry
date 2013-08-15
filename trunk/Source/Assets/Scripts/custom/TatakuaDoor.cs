using UnityEngine;
using System.Collections;

namespace BonusObjects
{

    public class TatakuaDoor : MonoBehaviour
    {


        void OnTriggerStay(Collider other)
        {

            if (other.CompareTag("Player") && (Managers.Game.InputUp)
                && Managers.Game.State != Managers.Game.GetComponentInChildren<HomeState>())
            {
                //Managers.Display.ShowFlash(.5f);
                //yield return new WaitForSeconds(0.5f);
                Managers.Game.PushState(typeof(HomeState));
            }

        }
    }

}