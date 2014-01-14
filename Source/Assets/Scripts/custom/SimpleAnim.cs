using UnityEngine;
using System.Collections;

namespace CustomAnimations
{

    public class SimpleAnim : MonoBehaviour
    {
        public enum AnimationType { Default, Bird, Fire, RaveLight, RaveGaucho1, RaveGaucho2, MateGaucho, Kurupi, Water, WaterAlpha}
        public AnimationType currentType = AnimationType.Default;

        public int RowStart = 0;
        public int ColumnStart = 0;
        public int TotalFrames = 1;
        public int Orientation = 1;
        //public int FramesPerSeconds = 12;


        private AnimSprite simpleAnim;
        private Transform thisTransform;
        Vector3 InitPos;

        void Start()
        {

            simpleAnim = gameObject.GetComponent<AnimSprite>() as AnimSprite;
            thisTransform = transform;
            InitPos = thisTransform.position;

            StartCoroutine(CoUpdate());                                 // Changed Because it can't hold re-activation
        }


        //void Update()
        //{
        //    switch (currentType)
        //    {
        //        case AnimationType.Default:
        //            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
        //            break;

        //        case AnimationType.Bird:
        //            simpleAnim.PlayFramesFixed(RowStart, ColumnStart, TotalFrames, Orientation);
        //            thisTransform.position += Vector3.up * Mathf.Sin(Time.time *3) * Time.deltaTime;
        //            break;
        //        case AnimationType.Fire:
        //            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
        //            counter += Time.deltaTime * 2;
        //            renderer.material.SetFloat("_KeyY", counter);

        //            counter %= 1;
        //            //counter = Mathf.Repeat(counter, 1);
        //            break;

        //        case AnimationType.Water:
        //            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
        //            break;
        //    }
        //}

        IEnumerator CoUpdate()
        {
            while (true)
            {

                switch (currentType)
                {
                    case AnimationType.Default:
                        simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
                        break;

                    case AnimationType.Bird:
                        simpleAnim.PlayFramesFixed(RowStart, ColumnStart, TotalFrames, Orientation);
                        thisTransform.position += Vector3.up * Mathf.Sin(Time.time *3) * Time.deltaTime;
                        break;
                    case AnimationType.Fire:
                        //simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
                        //counter += Time.deltaTime * 2;
                        //renderer.material.SetFloat("_KeyY", counter);

                        ////counter %= 1;
                        //counter = Mathf.Repeat(counter, 1);

                        simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
                        renderer.material.SetFloat("_KeyY", Managers.Display.PaletteSwap);

                        break;

                    case AnimationType.RaveLight:
                        simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation, 30);
                        thisTransform.RotateAround(InitPos + Vector3.up, Vector3.forward, Mathf.Sin(Time.time ) );
                        break;

                    case AnimationType.RaveGaucho1:
                            thisTransform.position += Vector3.right * Mathf.Sin(Time.time * 3) * 2 * Time.deltaTime;
                            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, (int)-Mathf.Sign(Mathf.Sin(Time.time * 3)));
                        break;

                    case AnimationType.RaveGaucho2:
                        if ( Mathf.Sin(Time.time ) > 0)
                            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, (int)Mathf.Sign(Mathf.Sin(Time.time * 3)), 9);
                        else
                            simpleAnim.PlayFrames(1, 0, 2, (int)Mathf.Sign(Mathf.Sin(Time.time * 5)), 9);

                        break;
                    case AnimationType.MateGaucho:
                        if (Managers.Register.Treasure2)
                        {
                             GameObject Gaucho = null;
                             if (Random.Range(0, 100) > 50)
                                 Gaucho = (GameObject)Instantiate(Resources.Load("Prefabs/Gaucho", typeof(GameObject)),
                                     transform.position, transform.rotation);
                             else
                                 Gaucho = (GameObject)Instantiate(Resources.Load("Prefabs/GauchoFacon", typeof(GameObject)),
                                     transform.position, transform.rotation);
							Gaucho.transform.parent = Managers.Tiled.MapTransform;
                             Destroy(gameObject);
                        }
                        else
                            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
                        break;

                    case AnimationType.Kurupi:

                        if (Managers.Game.PlayerPrefab != null)
                            if (Managers.Game.PlayerPrefab.transform.position.x > thisTransform.position.x)
                                Orientation = -1;
                            else
                                Orientation = 1;

                        if (Mathf.Sin(Time.time) > 0)
                            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
                        else
                            simpleAnim.PlayFrames(RowStart, ColumnStart +1, TotalFrames, Orientation);

                        if (Managers.Register.MboiTuiDefeated)
                            Destroy(thisTransform.gameObject);

                        break;

                    case AnimationType.Water:

                        renderer.material.mainTextureOffset = Managers.Display.WaterFlow;		// texture offset
                        renderer.material.mainTextureScale = new Vector3(.125f, .125f);		// texture scale

                        break;

                    case AnimationType.WaterAlpha:

                        renderer.material.mainTextureOffset = Managers.Display.WaterFlowAlpha;		// texture offset
                        renderer.material.mainTextureScale = new Vector3(.125f, .125f);		// texture scale

                        break;
                }

                yield return 0;
            }
        }


        void OnBecameVisible()
        {
            enabled = true;
        }

        void OnBecameInvisible()
        {
            enabled = false;
        }

        //void OnEnable()
        //{
        //    StartCoroutine(CoUpdate());                                 // Changed Because it can't hold re-activation
        //}
    }

}