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

    public double AddUpgrade(string name, double mul)
    {
        _range.AddUpgrade(name, mul);

        return _range.suckableRange;
    }

    public double AddAutoUpgrade(string name, double mul)
    {
        return 0;
    }

    public void Suck(Food food)
    {
        EffectManager.SuckingEffectPlay(suckEffector, food);
        
        statManager.AddFuel(food.containingFuel);
        statManager.AddMass(food.containingMass);
        statManager.AddTech(food.containingTech);
    }
}
