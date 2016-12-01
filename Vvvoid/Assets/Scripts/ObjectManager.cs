using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    [SerializeField] private float _startXPosition;
    [SerializeField] private float _yPositionMax;
    [SerializeField] private StatManager _statManager;
    [SerializeField] private Transform _removingPoint;
    [SerializeField] private GameObject _player;
    [SerializeField] private Sprite[] _prefabs;

    private List<GameObject> _resourcePool;
    private Queue<int> _freeObjectIndex;
    private float _elapsedTime = 0;
    private int _lastLevel;
    private int _prefabMaxLoadCount = 10;

    // Use this for initialization
    void Start ()
    {
        _resourcePool = new List<GameObject>();
        _freeObjectIndex = new Queue<int>();

        int scaleFactor = 1;
        for (int i = 0; i < _prefabs.Length; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                GameObject obj = new GameObject();
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = _prefabs[i];

                obj.transform.localScale *= scaleFactor;
                _resourcePool.Add(Instantiate(obj));
                obj.SetActive(false);
                _resourcePool.Add(obj);
            }
            scaleFactor++;
        }

        _lastLevel = (int)_statManager.maxScaleStep;
        for (int i = 0; i < _lastLevel; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                _freeObjectIndex.Enqueue(i * 10 + j);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if(_lastLevel < _statManager.maxScaleStep)
        {
            _lastLevel = (int)_statManager.maxScaleStep;

            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                _freeObjectIndex.Enqueue(_lastLevel * 10 + j);
            }
        }

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 1)
        {
            _elapsedTime = 0;
            if(_freeObjectIndex.Count > 0)
            {
                int newObjectIndex = _freeObjectIndex.Dequeue();
                GameObject meteor = _resourcePool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(_startXPosition, Random.Range(-_yPositionMax, _yPositionMax), 0);
            }
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
                        _statManager.AddResource(resource.containingResource);

                        obj.SetActive(false);
                        _freeObjectIndex.Enqueue(_resourcePool.IndexOf(obj));
                    }
                }
            }
        }

        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            foreach (var resource in _resourcePool)
            {
                resource.transform.localScale *= 2;

                float x = (resource.transform.position.x - -_player.transform.position.x) * 2;
                float y = (resource.transform.position.y - -_player.transform.position.y) * 2;
                resource.transform.position = new Vector3(x, y, resource.transform.position.z);
            }
            _player.transform.localScale *= 2;
            _statManager.ZoomInOutByStep(1);

        }
        else if (d < 0f)
        {
            foreach (var resource in _resourcePool)
            {
                resource.transform.localScale /= 2;
                float x = (resource.transform.position.x - -_player.transform.position.x) / 2;
                float y = (resource.transform.position.y - -_player.transform.position.y) / 2;
                resource.transform.position = new Vector3(x, y, resource.transform.position.z);
            }

            _player.transform.localScale /= 2;
            _statManager.ZoomInOutByStep(-1);
        }

        foreach (var resource in _resourcePool)
        {
            if(resource.activeSelf && resource.transform.position.x < _removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                resource.SetActive(false);
                _freeObjectIndex.Enqueue(_resourcePool.IndexOf(resource));
            }
            float scrollSpeed = _statManager.GetScrollSpeed();
            resource.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;
        }

    }
}
