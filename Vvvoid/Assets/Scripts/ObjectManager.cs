using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    public GameObject meteorPrefab = null;
    public float startXPosition;
    public float yPositionMax;
    public StatManager statManager;
    public Transform removingPoint;
    public GameObject player;

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

        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            foreach (var resource in resourcePool)
            {
                resource.transform.localScale *= 2;

                float x = (resource.transform.position.x - player.transform.position.x) * 2;
                float y = (resource.transform.position.y - player.transform.position.y) * 2;
                resource.transform.position = new Vector3(x, y, resource.transform.position.z);
            }
            player.transform.localScale *= 2;
            statManager.ZoomInOutByStep(1);

        }
        else if (d < 0f)
        {
            foreach (var resource in resourcePool)
            {
                resource.transform.localScale /= 2;
                float x = (resource.transform.position.x - player.transform.position.x) / 2;
                float y = (resource.transform.position.y - player.transform.position.y) / 2;
                resource.transform.position = new Vector3(x, y, resource.transform.position.z);
            }

            player.transform.localScale /= 2;
            statManager.ZoomInOutByStep(-1);
        }

        foreach (var resource in resourcePool)
        {
            if(resource.activeSelf && resource.transform.position.x < removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                resource.SetActive(false);
                freeObjectIndex.Enqueue(resourcePool.IndexOf(resource));
            }
            float scrollSpeed = statManager.GetScrollSpeed();
            resource.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;
        }

    }
}
