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
    [SerializeField]
    private GameObject DefulatRangeChecker;
    public float DefaultLocalScale;
    public double suckableRange { get; private set; }

    public List<SuckerUpgrade> upgradedList;
    
    void Start()
    {
        suckableRange = Math.Abs(DefulatRangeChecker.transform.position.x);
        transform.localScale = new Vector3(DefaultLocalScale, DefaultLocalScale, DefaultLocalScale);
    }

    public void AddUpgrade(string name, double multiply)
    {
        upgradedList.Add(new SuckerUpgrade(name, multiply));
        transform.localScale *= (float)multiply; //change appearnce size

        ApplyUpgrade();
    }

    void ApplyUpgrade()
    {
        suckableRange = DefulatRangeChecker.transform.position.z;
        foreach (SuckerUpgrade upgrade in upgradedList)
        {
            suckableRange *= upgrade.multiply;
        }
    }
    

}
