using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    [SerializeField] private Transform _startXTransform;
    [SerializeField] private Transform _startYTransform;
    [SerializeField] private StatManager _statManager;
    [SerializeField] private Transform _removingPoint;
    [SerializeField] private GameObject _player;
    [SerializeField] private Sprite[] _prefabs;

    private float _startXPosition;
    private float _startYPosition;
    private List<GameObject> _resourcePool;
    private Queue<int> _inactiveResourceIndexes;
    private List<int> _activeResourceIndexes;
    private float _elapsedTime = 0;
    private int _lastLevel;
    private int _prefabMaxLoadCount = 20;
    private int _preLoadingLevelInterval = 1;
    
    void Start ()
    {
        _resourcePool = new List<GameObject>();
        _inactiveResourceIndexes = new Queue<int>();
        _activeResourceIndexes = new List<int>();

        _startXPosition = _startXTransform.position.x;
        _startYPosition = _startYTransform.position.y;

        MakeObjectPool();

        _lastLevel = (int)_statManager.maxScaleStep;
        for (int i = 0; i <= _lastLevel + _preLoadingLevelInterval; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                int newObjectIndex = i * _prefabMaxLoadCount + j;
                GameObject meteor = _resourcePool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(Random.Range(-_startXPosition * 2, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                _activeResourceIndexes.Add(newObjectIndex);
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
            int currentLoadingLevel = _lastLevel + _preLoadingLevelInterval;
            if (currentLoadingLevel < _prefabs.Length)
            {
                List<int> indexes = new List<int>();
                foreach(var index in _inactiveResourceIndexes)
                {
                    indexes.Add(index);
                }
                _inactiveResourceIndexes.Clear();

                for (int i = _prefabMaxLoadCount * currentLoadingLevel + 1; i <= _prefabMaxLoadCount * (currentLoadingLevel + 1); i++)
                {
                    indexes.Add(i);
                }

                indexes.Sort((x, y) => Random.value < 0.5f ? -1 : 1);

                for (int i = 0; i < _prefabMaxLoadCount * (_lastLevel + 1); i++)
                {
                    _inactiveResourceIndexes.Enqueue(indexes[i]);
                }
            }
        }

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 1 / _statManager.GetScrollSpeed())
        {
            Debug.Log("On Level " + _statManager.currentScaleStep);
            Debug.Log("Max Level " + _statManager.maxScaleStep);
            _elapsedTime = 0;
            if(_inactiveResourceIndexes.Count > 0)
            {
                int newObjectIndex = _inactiveResourceIndexes.Dequeue();
                GameObject meteor = _resourcePool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(Random.Range(_startXPosition, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                _activeResourceIndexes.Add(newObjectIndex);
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

        UpdateObjectsScale();

    }

    void UpdateActiveObjects()
    {
        foreach (var index in _activeResourceIndexes)
        {
            GameObject obj = _resourcePool[index];
            Resource resource = obj.GetComponent<Resource>();
            if (resource.levelToReveal > _statManager.currentScaleStep + 1
                || resource.levelToReveal < _statManager.currentScaleStep - 1)
            {
                if (!resource.isExhausted)
                    obj.SetActive(false);
            }
            else
            {
                if (!resource.isExhausted)
                    obj.SetActive(true);
            }
        }
    }

    void UpdateObjectsScale()
    {
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            Debug.Log("D: " + d);
            foreach (var resource in _resourcePool)
            {
                MoveManager.MetorChangingScaleMove(resource, d, _player);
            }

            _player.transform.localScale *= 2;
            _statManager.ZoomInOutByStep(-1);
            UpdateActiveObjects();

        }
        else if (d < 0f)
        {
            Debug.Log("D: " + d);
            foreach (var resource in _resourcePool)
            {
                MoveManager.MetorChangingScaleMove(resource, d, _player);
            }

            _player.transform.localScale /= 2;
            _statManager.ZoomInOutByStep(1);
            UpdateActiveObjects();
        }

        foreach (var resource in _resourcePool)
        {
            if (resource.activeSelf && resource.transform.position.x < _removingPoint.position.x)
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
