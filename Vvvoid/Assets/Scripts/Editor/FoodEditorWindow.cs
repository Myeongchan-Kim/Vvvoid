using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class FoodEditorWindow : EditorWindow
{
    public class FoodInfo
    {
        public string foodName = "";
        public double baseFuelPoint = 0.0;
        public double baseMassPoint = 0.0;
        public double baseTechPoint = 0.0;
        public int maxScale = 100;
        public int minScale = 0;
    }

    const int levelGroupSize = 5;

    public int foodListSize = 10;
    public string saveFileName = ".";
    public double[] levelMultipleConst = new double[levelGroupSize];
    List<FoodInfo> foodInfoList = new List<FoodInfo>();


    void Awake()
    {
        // init List from file
        if (IsFoodDataFile(this.saveFileName))
        {
            LoadFood(this.saveFileName);
        }
        else
        {
            InitFood();
        }

    }
    void OnGUI()
    {       
        ShowFoodListOnWindow();
    }

    void ShowFoodListOnWindow()
    {

        GUILayout.Label("Foodlist stats");
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Realational stage");
        for(int i = 0; i < levelGroupSize; i++)
        {
            GUILayout.Label("" + i);
        }
        GUILayout.EndHorizontal();


        // multily constant edit.
        GUILayout.BeginHorizontal();
        GUILayout.Label("Multiply constant :");
        for(int i = 0; i < levelGroupSize; i++)
        {
            double newMul = EditorGUILayout.DoubleField(levelMultipleConst[i]);
            if (newMul != levelMultipleConst[i])
            {
                levelMultipleConst[i] = newMul;
                SaveFood(saveFileName);
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Food Name");
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
            double newFuel = EditorGUILayout.DoubleField(foodi.baseFuelPoint);
            double newMass = EditorGUILayout.DoubleField(foodi.baseMassPoint);
            double newTech = EditorGUILayout.DoubleField(foodi.baseTechPoint);

            EditorGUILayout.EndHorizontal();
            if (newFuel != foodi.baseFuelPoint ||
               newMass != foodi.baseMassPoint ||
               newTech != foodi.baseTechPoint )
            {
                foodi.baseFuelPoint = newFuel;
                foodi.baseMassPoint = newMass;
                foodi.baseTechPoint = newTech;
                SaveFood(saveFileName);
            }
        }
    }

    bool IsFoodDataFile(string fileName)
    {
        Debug.Log(" ========== Chekc!!");
        Debug.Log(PlayerPrefs.HasKey("baseFuel.0"));
        if (PlayerPrefs.HasKey("baseFuel.0"))
            return true;
        else
            return false;
    }

    void SaveFood(string fileName)
    {
        for( int i = 0; i < foodListSize; i++)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Length = 0;

            sb.AppendFormat("%f", foodInfoList[i].baseFuelPoint);
            PlayerPrefs.SetString("baseFuel.i", sb.ToString());
            sb.Length = 0;

            sb.AppendFormat("%f", foodInfoList[i].baseMassPoint);
            PlayerPrefs.SetString("baseMass.i", sb.ToString());
            sb.Length = 0;

            sb.AppendFormat("%f", foodInfoList[i].baseTechPoint);
            PlayerPrefs.SetString("baseTech.i", sb.ToString());
            sb.Length = 0;
        }
        PlayerPrefs.Save();
        Debug.Log("================== Data saved.");
    }

    void LoadFood(string fileName)
    {
        for (int i = 0; i < foodListSize; i++)
        {
            String val_str = PlayerPrefs.GetString(string.Format("baseFuel.{0}", i));
            double value = Convert.ToDouble(val_str);
            foodInfoList[i].baseFuelPoint = value;

            val_str = PlayerPrefs.GetString(string.Format("baseMass.{0}", i));
            value = Convert.ToDouble(val_str);
            foodInfoList[i].baseMassPoint = value;

            val_str = PlayerPrefs.GetString(string.Format("baseTech.{0}", i));
            value = Convert.ToDouble(val_str);
            foodInfoList[i].baseTechPoint = value;
        }
    }

    void InitFood()
    {
        levelMultipleConst = new double[5]{ 1,2,4,7,10 };

        for (int i = 0; i < foodListSize; i++)
        {
            FoodInfo foodi = new FoodInfo();
            foodi.foodName = "No." + i + " food";
            foodi.baseFuelPoint = Math.Pow(2, i);
            foodi.baseMassPoint = 0.0;
            foodi.baseTechPoint = foodi.baseFuelPoint * 0.5;
            foodInfoList.Add(foodi);
        }
    }

}
