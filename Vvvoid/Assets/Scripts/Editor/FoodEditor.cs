using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FoodManager))]
public class FoodEditor : Editor
{
    FoodEditorWindow window;

    void OnEnable()
    {
        FoodManager windowInfo = target as FoodManager;
        window = EditorWindow.GetWindow<FoodEditorWindow>("Food Editor");
        window.title  = "Food Editor";
        window.position = new Rect(20, 40, 500, 700);
        window.foodInfoList = windowInfo.FoodDatas;
        window.multiplyConstant = windowInfo.multiplyConstant;
        
    }

    void OnDisable()
    {
        window.Close();
    }
    
}
