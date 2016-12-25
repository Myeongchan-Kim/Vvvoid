using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

    public double containingFuel;
    public double containingTech;
    public double containingMass;
    public Vector3 standardPos;
    public int minScaleStep;
    public int maxScaleStep;
    public int standardScaleStep;
    public bool isExhausted;

    public Sprite sprite;
}