using UnityEngine;
using System.Collections;
using System;

public class Sucker : MonoBehaviour {
    [SerializeField]
    private StatManager statManager;

    [SerializeField]
    private ParticleSystem suckEffector;

    [SerializeField]
    private SuckerRange _range;

    public double GetRange()
    {
        float playerScale = transform.parent.localScale.x;
        Debug.Log("Current playerScale:" + playerScale);
        return _range.suckableRange * playerScale;
    }

    public void Suck(Food food)
    {
        EffectManager.SuckingEffectPlay(suckEffector, food);
        
        statManager.AddFuel(food.containingFuel);
        statManager.AddMass(food.containingMass);
        statManager.AddTech(food.containingTech);
    }
}
