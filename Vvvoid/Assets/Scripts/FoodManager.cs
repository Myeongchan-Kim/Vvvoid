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
    public int standardScale = 1;

    public Sprite sprite;

}

[System.Serializable]
public class StageMultiply
{
    public double scale;
    public double fuel;
    public double tech;
}


public class FoodManager : MonoBehaviour {

    public FoodInfo[] foodDatas;
    public StageMultiply[] multiplyConstant;
    public GameObject objman;

    public Food FillFoodInfoByLevel(int level, Food newFood)
    {
        FoodInfo foodInfo = foodDatas[level];
        newFood.name = foodInfo.foodName;
        newFood.minScaleStep = foodInfo.minScale;
        newFood.maxScaleStep = foodInfo.maxScale;
        newFood.standardScaleStep = foodInfo.standardScale;
        newFood.isExhausted = false;
        newFood.containingFuel = foodInfo.baseFuelPoint;
        newFood.containingTech = foodInfo.baseTechPoint;
        newFood.containingMass = foodInfo.baseMassPoint;
        newFood.sprite = foodInfo.sprite;

        return newFood;
    }
}
