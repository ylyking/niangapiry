using UnityEngine;
using System.Collections;

namespace BonusObjects
{

    public class DimensionalDoor : MonoBehaviour
    {
        public string NextWorld;

        void OnTriggerStay(Collider other)
        {

            if (other.CompareTag("Player") && (Managers.Game.InputUp)
                && Managers.Game.State != Managers.Game.GetComponentInChildren<DummyState>())
            {
                Input.ResetInputAxes();

                Managers.Display.ShowFlash(1);
                //Managers.Register.previousLevelFile = Managers.Register.currentLevelFile;
                Managers.Game.PushState(typeof(DummyState));

                Managers.Tiled.Unload();

                if (Managers.Tiled.Load(NextWorld))
                {
                    Input.ResetInputAxes();
                    return;
                }
            }

        }
    }

}