using UnityEngine;
using System.Collections;
using System;


[System.Serializable]
public class FoodInfo
{
    public string foodName = "";
    public double baseFuelPoint = 0.0;
    public double baseMassPoint = 0.0;
    public double baseTechPoint = 0.0;
    public int maxScale = 100;
    public int minScale = 0;

    public GameObject sprite;
}

[System.Serializable]
public class StageMultiply
{
    public double scale;
    public double fuel;
    public double tech;
}


public class Food : MonoBehaviour {

    public FoodInfo[] FoodDatas;
    public StageMultiply[] multiplyConstant;
    
}
