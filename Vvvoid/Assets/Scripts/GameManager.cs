using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Transform _startXTransform;
    [SerializeField] private Transform _startYTransform;
    [SerializeField] private StatManager _statManager;
    [SerializeField] private ObjectManager _objManager;
    [SerializeField] private Transform _removingPoint;
    [SerializeField] private GameObject _player;

    private float _startXPosition;
    private float _startYPosition;
    private float _elapsedTime = 0;
    private int _lastLevel;
    private int _prefabMaxLoadCount = 20;
    private int _preLoadingLevelInterval = 1;
    
    void Start ()
    {
        _objManager.MakeObjects();

        _lastLevel = (int)_statManager.maxScaleStep;
        _startXPosition = _startXTransform.position.x;
        _startYPosition = _startYTransform.position.y;


        for (int i = 0; i <= _lastLevel + _preLoadingLevelInterval; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                int newObjectIndex = i * _prefabMaxLoadCount + j;
                GameObject meteor = _objManager.ResourcePool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(Random.Range(-_startXPosition * 2, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                _objManager.ActiveResourceIndexes.Add(newObjectIndex);
            }
        }
    }
    void Update ()
    {
        if (_lastLevel < _statManager.maxScaleStep)
        {
            _lastLevel = (int)_statManager.maxScaleStep;
            int currentLoadingLevel = _lastLevel + _preLoadingLevelInterval;
            _objManager.LoadNewLevelObjects(currentLoadingLevel);
        }

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 1 / _statManager.GetScrollSpeed())
        {
            Debug.Log("On Level " + _statManager.currentScaleStep);
            Debug.Log("Max Level " + _statManager.maxScaleStep);
            _elapsedTime = 0;
            if(_objManager.InactiveResourceIndexes.Count > 0)
            {
                int newObjectIndex = _objManager.InactiveResourceIndexes.Dequeue();
                GameObject meteor = _objManager.ResourcePool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(Random.Range(_startXPosition, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                _objManager.ActiveResourceIndexes.Add(newObjectIndex);
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
                        _objManager.InactiveResourceIndexes.Enqueue(_objManager.GetIndexOfObject(obj));
                    }
                }
            }
        }

        UpdateObjectsScale();
        UpdateObjectPostition();

    }

    void UpdateActiveObjects()
    {
        foreach (var index in _objManager.ActiveResourceIndexes)
        {
            GameObject obj = _objManager.ResourcePool[index];
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
            foreach (var resource in _objManager.ResourcePool)
            {
                EffectManager.MetorUpScale(resource, _player);
            }

            EffectManager.ScaleChange(_player, 2.0f);
            _statManager.ZoomInOutByStep(-1);
            UpdateActiveObjects();

        }
        else if (d < 0f)
        {
            foreach (var resource in _objManager.ResourcePool)
            {
                EffectManager.MeteorDownScale(resource, _player);
            }

            EffectManager.ScaleChange(_player, 0.5f);
            _statManager.ZoomInOutByStep(1);
            UpdateActiveObjects();
        }
    }

    void UpdateObjectPostition()
    {
        foreach (var resource in _objManager.ResourcePool)
        {
            if (resource.activeSelf && resource.transform.position.x < _removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                resource.SetActive(false);
                int index = _objManager.ResourcePool.IndexOf(resource);
                _objManager.InactiveResourceIndexes.Enqueue(index);
                _objManager.ActiveResourceIndexes.Remove(index);
            }
            float scrollSpeed = _statManager.GetScrollSpeed();
            resource.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;
        }
    }

}
