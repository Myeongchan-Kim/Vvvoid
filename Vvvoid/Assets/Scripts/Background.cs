using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

    public RectTransform background1;
    public RectTransform background2;
    public StatManager statManger;
    private Vector3 startPosition1;
    private Vector3 startPosition2;

    // Use this for initialization
    void Start () {
        startPosition1 = background1.transform.position;
        startPosition2 = background2.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        float newPosition = Mathf.Repeat((float)statManger.distance, background1.rect.width);
        background1.transform.position = startPosition1 - new Vector3(1, 0, 0) * newPosition;
        background2.transform.position = startPosition2 - new Vector3(1, 0, 0) * newPosition;
    }
}
