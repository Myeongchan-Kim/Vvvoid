using UnityEngine;
using System.Collections;
using System;

public class Sucker : MonoBehaviour {


    [SerializeField] private StatManager _statManager;
    [SerializeField] private ParticleSystem _suckEffector;
    [SerializeField] private SuckerRange _manualSuckerRange;
    [SerializeField] private SuckerRange _autoSuckerRange;

    public double GetManualSuckerRange()
    {
        float playerScale = transform.parent.localScale.x;
        return _manualSuckerRange.suckableRange * playerScale;
    }

    public double GetAutoSuckerRange()
    {
        float playerScale = transform.parent.localScale.x;
        return _autoSuckerRange.suckableRange * playerScale;
    }

    public void Suck(Food food)
    {
        EffectManager.SuckingEffectPlay(_suckEffector, food);

        _statManager.AddFuel(food.containingFuel);
        _statManager.AddMass(food.containingMass);
        _statManager.AddTech(food.containingTech);
    }
}
