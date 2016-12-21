using UnityEngine;
using System;

public class GameManager : MonoBehaviour {
    [SerializeField] private StatManager _statManager;
    [SerializeField] private ObjectManager _objManager;
    [SerializeField] private Transform _removingPoint;
    [SerializeField] private GameObject _playerObj;

    private float _elapsedTime = 0;
    private int _currentLevel;
    private int _prefabMaxLoadCount = 20;
    
    void Start ()
    {
        _objManager.MakeObjectPool();

        _currentLevel = (int)_statManager.MaxScaleStep;


        _objManager.LoadNewLevelObjects(_currentLevel);

        double currentScale = _statManager.CurrentScaleStep;
        ApplyCurrentScale(currentScale, currentScale);
    }

    void Update ()
    {
        //Load New Level Objects
        if (_currentLevel < _statManager.MaxScaleStep)
        {
            _currentLevel = (int)_statManager.MaxScaleStep;

            _objManager.LoadNewLevelObjects(_currentLevel);
        }

        //Generate new food
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 1 / _statManager.GetScrollSpeed())
        {
            _elapsedTime = 0;
            _objManager.SpawnNewFood();
        }

        CheckExaustedFoods();
        UpdateObjectsScale();
        UpdateObjectPostition();

    }
     

    void CheckExaustedFoods()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hitInfo)
            {
                HitProceedure(hitInfo);
            }
        }
    }

    #region CheckExausteFoods related funcs
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
        Sucker sucker = _playerObj.GetComponentInChildren<Sucker>();
        Food food = obj.GetComponent<Food>();


        float SuckRange = (float)sucker.GetRange();
        float dist = Vector3.Distance(hitInfo.point, transform.position);
        Debug.Log("Hit info: " + hitInfo.point);
        Debug.Log("Dist: " + dist);

        if (obj.activeSelf && dist < SuckRange)
        {
            // suck procedure
            // fuel+, tech+, mass+
            SuckFoodProcess(sucker, food);

            _objManager.InactiveFoodIndexQueue.Enqueue(_objManager.GetIndexOfObject(obj));
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

#endregion

    void UpdateActiveObjects()
    {
        foreach (var index in _objManager.ActiveFoodIndexes)
        {
            GameObject obj = _objManager.FoodPool[index];
            Food food = obj.GetComponent<Food>();
            if (food.levelToReveal > _statManager.CurrentScaleStep - 1
               || food.levelToReveal < _statManager.CurrentScaleStep + 1)
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
        double currentScale = _statManager.CurrentScaleStep;
        if (d > 0f)
        {
            double newScaleStep = _statManager.ZoomInOutByStep(1);
            ApplyCurrentScale(currentScale, newScaleStep);
            UpdateActiveObjects();

        }
        else if (d < 0f)
        {
            double newScaleStep = _statManager.ZoomInOutByStep(-1);
            ApplyCurrentScale(currentScale, newScaleStep);
            UpdateActiveObjects();
        }
    }

    void UpdateObjectPostition()
    {
        foreach (var food in _objManager.FoodPool)
        {
            if (food.activeSelf && food.transform.position.x < _removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                food.SetActive(false);
                int index = _objManager.FoodPool.IndexOf(food);
                _objManager.InactiveFoodIndexQueue.Enqueue(index);
                _objManager.ActiveFoodIndexes.Remove(index);
            }
            float scrollSpeed = _statManager.GetScrollSpeed();
            food.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;
            Food f = food.GetComponent<Food>();
            f.standardPos -= new Vector3(scrollSpeed / (float)Math.Pow(2, f.standardScaleStep - _statManager.CurrentScaleStep), 0, 0) * Time.deltaTime;
        }
    }

    void ApplyCurrentScale(double oldScale, double newScaleStep)
    {
        Player player = _playerObj.GetComponent<Player>();

        //every object has its own scalestep. 
        double standardScaleStep = player.standardScaleStep;
        float newLocalScaleOfPlayer = (float)Math.Pow(2, standardScaleStep - newScaleStep);

        // Debug.Log("===== Oldscale:" + oldScale + " NewScale:" + newScaleStep + "standard:" + standardScaleStep);
        EffectManager.ScaleChange(_playerObj, newLocalScaleOfPlayer);

        foreach (var food in _objManager.FoodPool)
        {
            Food f = food.GetComponent<Food>();

            // Change scale of Food
            float newLocalScaleOfFood = (float)Math.Pow(2, f.standardScaleStep - newScaleStep);
            EffectManager.ScaleChange(food, newLocalScaleOfFood);

            // Change position of Food
            float newPostionScaleOfFood = (float)Math.Pow(2, (f.standardScaleStep - newScaleStep));
            Vector3 newPos = f.standardPos * newPostionScaleOfFood;
            EffectManager.ChangeFoodPositon(food, _playerObj, newPos);
        }
    }


}
