using UnityEngine;
using System.Collections;

public class SimpleAnim : MonoBehaviour
{
    public int RowStart = 0;
    public int ColumnStart = 0;
    public int TotalFrames = 1;
    public int Orientation = 1;
    //public int FramesPerSeconds = 12;

    private AnimSprite simpleAnim;

    void Start(){

        simpleAnim = gameObject.GetComponent<AnimSprite>()as AnimSprite;

        //StartCoroutine(CoUpdate());                                 // Changed Because it can't hold re-activation
}
    void Update()
    {
            simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
    }

    //IEnumerator CoUpdate()
    //{
    //    while (true)
    //    {
    //        simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
    //        yield return 0;
    //    }
    //}
}