using UnityEngine;
using System.Collections;

public class ParticleCleaning : MonoBehaviour {

    // This it's a fix for the stupid change on new Unity's Particle System to not included an auto-Destroy
    //http://blog.i-evaluation.com/2013/02/17/auto-destroying-particle-system-in-unity3d/

    void Awake()
    {
        var particleSystem = this.gameObject.GetComponent<ParticleSystem>();

        Destroy(this.gameObject, particleSystem.duration + particleSystem.startLifetime + 5);
    }
}
