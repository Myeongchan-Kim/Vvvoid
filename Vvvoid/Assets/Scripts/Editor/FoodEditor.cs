using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Food))]
public class FoodEditor : Editor
{
    FoodEditorWindow window;

    void OnEnable()
    {
        Debug.Log("OnEnable");
        window = EditorWindow.GetWindow<FoodEditorWindow>("Food Editor");
        //window.title  = Meuw1;
    }

    void OnDisable()
    {
        window.Close();
    }
    
}
