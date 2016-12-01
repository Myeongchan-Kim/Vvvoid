using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectFactory : MonoBehaviour {

    [SerializeField]private Sprite[] prefabs;
    [SerializeField]public StatManager statManger;

    public List<GameObject> objectPool;
    public Queue<int> indexes;
    public int lastLevel;

    void Start () {
        objectPool = new List<GameObject>();

        for(int i = 0; i < prefabs.Length; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                GameObject obj = new GameObject();
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = prefabs[i];
                objectPool[i + j] = Instantiate(obj);
            }
        }
        lastLevel = (int)statManger.maxScaleStep;

	}
	
	// Update is called once per frame
	void Update () {
	    if(lastLevel < (int)statManger.currentScaleStep)
        {
            lastLevel = (int)statManger.currentScaleStep;
        }
        SetIndexes();
	}

    void SetIndexes()
    {
        if(lastLevel > 1)
        {
            for (int i = lastLevel - 1; i <= lastLevel + 1; i++)
            {
            }
        }
    }
}
