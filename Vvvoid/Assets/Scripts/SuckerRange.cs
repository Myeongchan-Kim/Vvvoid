using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SuckerUpgrade
{
    public SuckerUpgrade(string name, double multiply)
    {
        this.name = name;
        this.multiply = multiply;
    }

    public string name;
    public double multiply;
}

public class SuckerRange : MonoBehaviour {


    [SerializeField] private GameObject _rangeChecker;

    public float defaultLocalScale;

    public double SuckableRange { get; private set; }

    public List<SuckerUpgrade> upgradedList;
    
    void Start()
    {   SuckableRange = Math.Abs(_rangeChecker.transform.position.x);
        transform.localScale = new Vector3(defaultLocalScale, defaultLocalScale, defaultLocalScale);
    }

    public void AddUpgrade(string name, double multiply)
    {
        upgradedList.Add(new SuckerUpgrade(name, multiply));
        transform.localScale *= (float)multiply; //change appearnce size

        ApplyUpgrade();
    }

    void ApplyUpgrade()
    {
        SuckableRange = _rangeChecker.transform.position.z;
        foreach (SuckerUpgrade upgrade in upgradedList)
        {
            SuckableRange *= upgrade.multiply;
        }
    }
    

}
