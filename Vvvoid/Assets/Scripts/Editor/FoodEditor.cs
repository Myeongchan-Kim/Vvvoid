using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FoodManager))]
public class FoodEditor : Editor
{
    FoodEditorWindow window;

    void OnEnable()
    {
        FoodManager foodmanager = target as FoodManager;
        window = EditorWindow.GetWindow<FoodEditorWindow>("Food Editor");
        window.title  = "Food Editor";
        window.position = new Rect(20, 40, 500, 700);
        window.foodInfoList = foodmanager.foodDatas;
        window.multiplyConstant = foodmanager.multiplyConstant;
        window.objMan = foodmanager.objman.GetComponent<ObjectManager>();
    }

    void OnDisable()
    {
        window.Close();
    }
    
}
