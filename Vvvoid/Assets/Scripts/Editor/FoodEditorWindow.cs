using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class FoodEditorWindow : EditorWindow
{
    //public int levelGroupSize;
    public int foodListSize;
    public string saveFileName = ".";

    public StageMultiply[] multiplyConstant;
    public FoodInfo[] foodInfoList;

    public ObjectManager objMan;

    void OnGUI()
    {
        ShowInitButton();
        ShowFoodListOnWindow();
    }

    void ShowInitButton()
    {
        if (GUILayout.Button("init"))
        {
            InitFood();
            InitMultiply();
        }
    }

    void InitMultiply()
    {
        
    }

    void ShowFoodListOnWindow()
    {
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Realational mul:");
        for (int i = 0; i < multiplyConstant.GetLength(0); i++)
        {
            GUILayout.Label("" + i + " level");
        }
        GUILayout.EndHorizontal();

        // multily constant edit.
        //Scale
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tech multiply:");
        foreach (StageMultiply mul in multiplyConstant)
        {
            mul.scale = EditorGUILayout.DoubleField(mul.scale);
        }
        GUILayout.EndHorizontal();

        //fuel
        GUILayout.BeginHorizontal();
        GUILayout.Label("Fuel multiply:");
        foreach (StageMultiply mul in multiplyConstant)
        {
            mul.fuel = EditorGUILayout.DoubleField(mul.fuel);
        }
        GUILayout.EndHorizontal();

        //Tech
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tech multiply:");
        foreach (StageMultiply mul in multiplyConstant)
        {
            mul.tech = EditorGUILayout.DoubleField(mul.tech);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Food Name");
        GUILayout.Label("Standard");
        GUILayout.Label("Min stage");
        GUILayout.Label("Max stage");
        GUILayout.Label("Base Fuel");
        GUILayout.Label("Base Mass");
        GUILayout.Label("Base Tech");
        GUILayout.EndHorizontal();

        // each food status editor.
        foreach (FoodInfo foodi in foodInfoList)
        {

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(foodi.foodName);
            GUILayout.Space(10);

            foodi.standardScale = EditorGUILayout.IntField(foodi.standardScale);
            foodi.minScale = EditorGUILayout.IntField(foodi.minScale);
            foodi.maxScale = EditorGUILayout.IntField(foodi.maxScale);
            foodi.baseFuelPoint = EditorGUILayout.DoubleField(foodi.baseFuelPoint);
            foodi.baseMassPoint = EditorGUILayout.DoubleField(foodi.baseMassPoint);
            foodi.baseTechPoint = EditorGUILayout.DoubleField(foodi.baseTechPoint);

            EditorGUILayout.EndHorizontal();
        }
    }

    bool IsFoodDataFile(string fileName)
    {
        return true;
    }


    void InitFood()
    {
        for (int i = 0; i < foodInfoList.Length; i++)
        {
            FoodInfo foodi = foodInfoList[i];
            foodi.foodName = "No." + i + " food";
            foodi.baseFuelPoint = Math.Pow(2, i);
            foodi.baseMassPoint = 0.0;
            foodi.baseTechPoint = foodi.baseFuelPoint * 0.5;
            //foodi.sprite = objMan.GetSprite(i % 10);
            foodi.minScale = i - 2;
            foodi.maxScale = i + 8;
            foodi.standardScale = i + 1;
        }
    }
}
