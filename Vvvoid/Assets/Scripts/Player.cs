using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField]
    private ParticleSystem _rocketEffect;
    [SerializeField]
    private ParticleSystem _noFuelEffect;

    public double standardScaleStep = 0.0f;

    public void FireRocketEffect()
    {
        _rocketEffect.Play();
    }
    public void NoFuelEffect()
    {
        _noFuelEffect.Play();
    }
}
