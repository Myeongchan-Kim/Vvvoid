using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    public GameObject meteorPrefab = null;
    public float startXPosition;
    public float yPositionMax;
    public StatManager statManager;
    public BoxCollider cullingBox;

    private List<GameObject> resourcePool;
    private float elapsedTime = 0;
    private Queue<int> freeObjectIndex;

	// Use this for initialization
	void Start () {
        resourcePool = new List<GameObject>();
        freeObjectIndex = new Queue<int>();
        for(int i = 0; i < 30; i++)
        {
            GameObject obj = Instantiate<GameObject>(meteorPrefab);
            obj.SetActive(false);
            resourcePool.Add(obj);
            freeObjectIndex.Enqueue(i);
        }
	}
	
	// Update is called once per frame
	void Update () {

        elapsedTime += Time.deltaTime;
        if (elapsedTime > 1)
        {
            elapsedTime = 0;
            int newObjectIndex = freeObjectIndex.Dequeue();
            GameObject meteor = resourcePool[newObjectIndex];
            meteor.SetActive(true);
            meteor.transform.position = new Vector3(startXPosition, Random.Range(-yPositionMax, yPositionMax), 0);
            resourcePool.Add(meteor);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Resource")
                {
                    GameObject obj = hit.transform.gameObject;
                    if (obj.activeSelf)
                    {
                        Debug.Log("Hit");
                        Resource resource = obj.GetComponent<Resource>();
                        statManager.AddResource(resource.containingResource);

                        obj.SetActive(false);
                        freeObjectIndex.Enqueue(resourcePool.IndexOf(obj));
                    }
                }
            }
        }

        foreach (var meteor in resourcePool)
        {
            if(!cullingBox.bounds.Contains(meteor.transform.position))
            {
                meteor.SetActive(false);
                freeObjectIndex.Enqueue(resourcePool.IndexOf(meteor));
            }
            meteor.transform.position -= new Vector3(0.1f, 0, 0);
        }

    }
}
