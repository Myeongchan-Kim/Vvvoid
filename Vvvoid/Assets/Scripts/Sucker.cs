﻿using UnityEngine;
using System.Collections;

public class Sucker : MonoBehaviour {
    [SerializeField]
    private StatManager statManager;

    [SerializeField]
    private ParticleSystem suckEffector;

    [SerializeField]
    private SuckerRange _range;

    public double GetRange()
    {
        return _range.suckableRange;
    }

    public void Suck(Food food)
    {
        EffectManager.SuckingEffectPlay(suckEffector, food);
        
        statManager.AddFuel(food.containingFuel);
        statManager.AddMass(food.containingMass);
        statManager.AddTech(food.containingTech);
    }
}
