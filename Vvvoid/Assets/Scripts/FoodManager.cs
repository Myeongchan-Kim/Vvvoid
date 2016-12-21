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

    public Food FillFoodInfoByIndex(int index, Food newFood)
    {
        FoodInfo foodi = foodDatas[index];

        newFood.name = foodi.foodName;
        newFood.levelToReveal = foodi.minScale;
        newFood.standardScaleStep = ( foodi.minScale + 5);
        newFood.isExhausted = false;
        newFood.containingFuel = foodi.baseFuelPoint;
        newFood.containingTech = foodi.baseTechPoint;
        newFood.containingMass = foodi.baseMassPoint;
        newFood.spirte = foodi.sprite;

        return newFood;
    }
}
