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
    private Queue<int> _inactiveResourceIndexes;
    private List<int> _activeResourceIndexes;
    private float _elapsedTime = 0;
    private int _lastLevel;
    private int _prefabMaxLoadCount = 10;
    
    void Start ()
    {
        _resourcePool = new List<GameObject>();
        _inactiveResourceIndexes = new Queue<int>();
        _activeResourceIndexes = new List<int>();

        MakeObjectPool();

        _lastLevel = (int)_statManager.maxScaleStep;
        for (int i = 0; i <= _lastLevel; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                _inactiveResourceIndexes.Enqueue(i * _prefabMaxLoadCount + j);
            }
        }
    }

    void MakeObjectPool()
    {
        int scaleFactor = 1;
        for (int i = 0; i < _prefabs.Length; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                GameObject obj = new GameObject();
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = _prefabs[i];
                Resource resource = obj.AddComponent<Resource>();
                resource.levelToReveal = i;
                resource.isExhausted = false;
                resource.containingResource = i;
                BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

                obj.transform.localScale *= scaleFactor;
                obj.SetActive(false);
                _resourcePool.Add(obj);
                obj.name = "level " + i + " resource";
                obj.transform.tag = "Resource";
            }
            scaleFactor++;
        }
    }
    
    void Update ()
    {
        if (_lastLevel < _statManager.maxScaleStep)
        {
            _lastLevel = (int)_statManager.maxScaleStep;

            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                _inactiveResourceIndexes.Enqueue(_lastLevel * _prefabMaxLoadCount + j);
            }
        }

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 1)
        {
            Debug.Log("On Level " + _statManager.currentScaleStep);
            Debug.Log("Max Level " + _statManager.maxScaleStep);
            _elapsedTime = 0;
            if(_inactiveResourceIndexes.Count > 0)
            {
                int newObjectIndex = _inactiveResourceIndexes.Dequeue();
                GameObject meteor = _resourcePool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(_startXPosition, Random.Range(-_yPositionMax, _yPositionMax), 0);
                _activeResourceIndexes.Add(newObjectIndex);
            }
        }

        foreach(var index in _activeResourceIndexes)
        {
            GameObject obj = _resourcePool[index];
            Resource resource = obj.GetComponent<Resource>();
            if (resource.levelToReveal > _statManager.currentScaleStep + 1 
                || resource.levelToReveal < _statManager.currentScaleStep - 1)
            {
                if(!resource.isExhausted)
                    obj.SetActive(false);
            }
            else
            {
                if (!resource.isExhausted)
                    obj.SetActive(true);
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hitInfo)
            {
                if (hitInfo.transform.tag == "Resource")
                {
                    GameObject obj = hitInfo.transform.gameObject;
                    if (obj.activeSelf)
                    {
                        Debug.Log("Hit");
                        Resource resource = obj.GetComponent<Resource>();
                        _statManager.AddResource(resource.containingResource);
                        resource.isExhausted = true;

                        obj.SetActive(false);
                        _inactiveResourceIndexes.Enqueue(_resourcePool.IndexOf(obj));
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

                float x = (resource.transform.position.x - _player.transform.position.x) * 2;
                float y = (resource.transform.position.y - _player.transform.position.y) * 2;
                resource.transform.position = new Vector3(x, y, resource.transform.position.z);
            }
            _player.transform.localScale *= 2;
            _statManager.ZoomInOutByStep(-1);

        }
        else if (d < 0f)
        {
            foreach (var resource in _resourcePool)
            {
                resource.transform.localScale /= 2;
                float x = (resource.transform.position.x - _player.transform.position.x) / 2;
                float y = (resource.transform.position.y - _player.transform.position.y) / 2;
                resource.transform.position = new Vector3(x, y, resource.transform.position.z);
            }

            _player.transform.localScale /= 2;
            _statManager.ZoomInOutByStep(1);
        }

        foreach (var resource in _resourcePool)
        {
            if(resource.activeSelf && resource.transform.position.x < _removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                resource.SetActive(false);
                int index = _resourcePool.IndexOf(resource);
                _inactiveResourceIndexes.Enqueue(index);
                _activeResourceIndexes.Remove(index);
            }
            float scrollSpeed = _statManager.GetScrollSpeed();
            resource.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;
        }

    }
}
