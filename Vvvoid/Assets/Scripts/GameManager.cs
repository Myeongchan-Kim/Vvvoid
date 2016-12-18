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
                GameObject meteor = _objManager.foodPool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(Random.Range(-_startXPosition * 2, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                _objManager.ActiveFoodIndexes.Add(newObjectIndex);
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
            //Debug.Log("On Level " + _statManager.currentScaleStep);
            //Debug.Log("Max Level " + _statManager.maxScaleStep);
            _elapsedTime = 0;
            if(_objManager.InactiveFoodIndexes.Count > 0)
            {
                int newObjectIndex = _objManager.InactiveFoodIndexes.Dequeue();
                GameObject meteor = _objManager.foodPool[newObjectIndex];
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(Random.Range(_startXPosition, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                _objManager.ActiveFoodIndexes.Add(newObjectIndex);
            }
        }

        
        
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hitInfo)
            {
                HitProceedure(hitInfo);
            }
        }

        UpdateObjectsScale();
        UpdateObjectPostition();

    }

    void UpdateActiveObjects()
    {
        foreach (var index in _objManager.ActiveFoodIndexes)
        {
            GameObject obj = _objManager.foodPool[index];
            Food food = obj.GetComponent<Food>();
            if (food.levelToReveal > _statManager.currentScaleStep + 1
                || food.levelToReveal < _statManager.currentScaleStep - 1)
            {
                if (!food.isExhausted)
                    obj.SetActive(false);
            }
            else
            {
                if (!food.isExhausted)
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
            foreach (var food in _objManager.foodPool)
            {
                EffectManager.MetorUpScale(food, _player);
            }

            EffectManager.ScaleChange(_player, 2.0f);
            _statManager.ZoomInOutByStep(-1);
            UpdateActiveObjects();

        }
        else if (d < 0f)
        {
            foreach (var food in _objManager.foodPool)
            {
                EffectManager.MeteorDownScale(food, _player);
            }

            EffectManager.ScaleChange(_player, 0.5f);
            _statManager.ZoomInOutByStep(1);
            UpdateActiveObjects();
        }
    }

    void UpdateObjectPostition()
    {
        foreach (var food in _objManager.foodPool)
        {
            if (food.activeSelf && food.transform.position.x < _removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                food.SetActive(false);
                int index = _objManager.foodPool.IndexOf(food);
                _objManager.InactiveFoodIndexes.Enqueue(index);
                _objManager.ActiveFoodIndexes.Remove(index);
            }
            float scrollSpeed = _statManager.GetScrollSpeed();
            food.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;
        }
    }

    void HitProceedure(RaycastHit2D hitInfo)
    {
        if (hitInfo.transform.tag == "Food")
        {
            HitFood(hitInfo);
        }
        // .. else if ...
    }

    void HitFood(RaycastHit2D hitInfo)
    {
        GameObject obj = hitInfo.transform.gameObject;
        Sucker sucker = _player.GetComponentInChildren<Sucker>();
        Food food = obj.GetComponent<Food>();

        float SuckRange = (float)sucker.GetRange();
        float dist = Vector3.Distance(obj.transform.position, transform.position);

        if (obj.activeSelf && dist < SuckRange)
        {
            // suck procedure
            // fuel+, tech+, mass+
            SuckFoodProcess(sucker, food);

            _objManager.InactiveFoodIndexes.Enqueue(_objManager.GetIndexOfObject(obj));
        }
        else
        {
            Observe(obj);
        }
    }

    void Observe(GameObject obj)
    {
        //... observe proceedure

        // only tech+
    }

    void SuckFoodProcess(Sucker sucker, Food food)
    {
        Debug.Log("== Suck!");
        food.isExhausted = true;
        sucker.Suck(food);
    }
}
