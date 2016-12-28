using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    [SerializeField] private StatManager _statManager;
    [SerializeField] private ObjectManager _objManager;
    [SerializeField] private Transform _removingPoint;
    [SerializeField] private GameObject _playerObj;

    private float _elapsedTime = 0;
    private int _currentMaxLevel;
    
    void Start ()
    {
        _objManager.MakeObjectPool();
        _currentMaxLevel = (int)_statManager.MaxScaleStep;
        //_objManager.LoadInitialLevel(_currentMaxLevel);

        double currentScale = _statManager.CurrentScaleStep;
        ApplyCurrentScaleToAllFood(currentScale, currentScale);
        UpdateActiveObjects();
    }

    void Update ()
    {
        //Load New Level Objects
        if (_currentMaxLevel < _statManager.MaxScaleStep)
        {
            _currentMaxLevel = (int)_statManager.MaxScaleStep;
            _objManager.LoadNewLevelObjects(_currentMaxLevel);
        }

        //Generate new food
        _elapsedTime += Time.deltaTime;
        double density = 1;
        if (_elapsedTime > density / _statManager.GetScrollSpeed())
        {
            _elapsedTime = 0;
            _objManager.SpawnNewFood(_statManager.CurrentScaleStep);
        }

        CheckExaustedFoods();

        //Wheel input value from User.
        float d = Input.GetAxis("Mouse ScrollWheel");
        UpdateObjectsScale(d);

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

            if (food.isExhausted
                || _statManager.CurrentScaleStep < food.minScaleStep
                || _statManager.CurrentScaleStep > food.maxScaleStep)
                obj.SetActive(false);
        }
    }

    void UpdateObjectsScale(float d)
    {
        double currentScale = _statManager.CurrentScaleStep;
        double newScaleStep = currentScale;

        
        if (d > 0f)
        {
            newScaleStep = _statManager.ZoomInOutByStep(1);
            if (currentScale < newScaleStep)
            {
                ApplyCurrentScaleToAllFood(currentScale, newScaleStep);
                // Update Active Object after scaling..
                UpdateActiveObjects();
            }
        }else if (d < 0f)
        {
            newScaleStep = _statManager.ZoomInOutByStep(-1);
            if (currentScale > newScaleStep)
            {
                ApplyCurrentScaleToAllFood(currentScale, newScaleStep);

                // Update Active Object after scaling..
                UpdateActiveObjects();
            }
        }

    }

    void UpdateObjectPostition()
    {
        List<int> IndexListOutOfRange = new List<int>();
        foreach (var index in _objManager.ActiveFoodIndexes)
        {
            GameObject foodObj = _objManager.FoodPool[index];
            Food food = foodObj.GetComponent<Food>();

            if (food.standardPos.x < _removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                IndexListOutOfRange.Add(index);
                continue; 
            }

            float scrollSpeed = _statManager.GetScrollSpeed();
            foodObj.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;

            float standardScrollSpeed = scrollSpeed * (float)Math.Pow(2d, _statManager.CurrentScaleStep - food.standardScaleStep);
            food.standardPos -= new Vector3(standardScrollSpeed, 0, 0) * Time.deltaTime;
        }

        foreach (var i in IndexListOutOfRange)
        {
            GameObject food = _objManager.FoodPool[i];
            food.SetActive(false);
            _objManager.InactiveFoodIndexQueue.Enqueue(i);
            _objManager.ActiveFoodIndexes.Remove(i);
        }

        IndexListOutOfRange.Clear();
    }

    void ApplyCurrentScaleToAllFood(double oldScale, double newScaleStep)
    {
        Player player = _playerObj.GetComponent<Player>();

        //every object has its own scalestep. 
        double standardScaleStep = player.standardScaleStep;
        float newLocalScaleOfPlayer = (float)Math.Pow(2, standardScaleStep - newScaleStep);

        // Debug.Log("===== Oldscale:" + oldScale + " NewScale:" + newScaleStep + "standard:" + standardScaleStep);
        EffectManager.ScaleChange(_playerObj, newLocalScaleOfPlayer);

        foreach( int activeIndex in _objManager.ActiveFoodIndexes)
        {
            GameObject foodObj = _objManager.FoodPool[activeIndex];
            ApplyScaleForFood(foodObj, newScaleStep);
        }
        
    }

    void ApplyScaleForFood(GameObject foodObj, double newScaleStep)
    {
        Food f = foodObj.GetComponent<Food>();

        // Change scale of Food
        float newLocalScaleOfFood = (float)Math.Pow(2, f.standardScaleStep - newScaleStep);
        EffectManager.ScaleChange(foodObj, newLocalScaleOfFood);

        // Change position of Food
        float newPostionScale = (float)Math.Pow(2, (f.standardScaleStep - newScaleStep));
        Vector3 newPos = f.standardPos * newPostionScale;
        EffectManager.ChangeFoodPositon(foodObj, _playerObj, newPos);
    }

}
