using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    [SerializeField] private StatManager _statManager;
    [SerializeField] private ObjectManager _objManager;
    [SerializeField] private Transform _removingPoint;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private Sucker _sucker;

    
    private float _elapsedTime = 0.0f;
    private int _currentMaxLevel;
    private int _autoSuckCheckIndex = 0;
    
    void Start ()
    {
        _objManager.MakeObjectPool();
        _currentMaxLevel = (int)_statManager.MaxScaleStep;

        double currentScale = _statManager.CurrentScaleStep;
        ApplyCurrentScaleToAllFood(currentScale, currentScale);
        UpdateActiveObjects();
    }

    void Update ()
    {
        //Generate new food
        _elapsedTime += Time.deltaTime;
        double density = 1;
        if (_elapsedTime > density / _statManager.GetScrollSpeed())
        {
            _elapsedTime = 0;
            _objManager.SpawnNewFood(_statManager.CurrentScaleStep);
        }

        CheckSuckableFoods();

        //Wheel input value from User.
        float d = Input.GetAxis("Mouse ScrollWheel");
        UpdateObjectsScale(d);

        UpdateObjectPostition();

    }

    void CheckSuckableFoods()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckManualSuckerRange();
        }
        
        CheckAutoSuckerRange();
    }

    void CheckManualSuckerRange()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hitInfo)
        {
            if (hitInfo.transform.tag == "Food")
            {
                HitFood(hitInfo);
                return;
            }
        }

        double consumedEnergy = _statManager.AccelCharacter();
        if (consumedEnergy > 0.0)
        {
            _statManager.AnimateAccel(consumedEnergy);
        }
        else
        {
            _statManager.AnimateNoFuel();
        }
    }

    void CheckAutoSuckerRange()
    {
        foreach (var index in _objManager.MovingFoodIndexes)
        {
            GameObject obj = _objManager.FoodPool[index];
            if (obj.transform.position.x < _playerObj.transform.position.x + _sucker.GetAutoSuckerRange())
            {
                Food food = obj.GetComponent<Food>();
                if (!food.isExhausted)
                {
                    if (food.minScaleStep < _statManager.MaxScaleStep)
                    {

                        float suckRange = (float)_sucker.GetAutoSuckerRange();
                        float dist = Vector3.Distance(obj.transform.position, _playerObj.transform.position);
                        if (dist < suckRange)
                        {
                            SuckFoodProcess(_sucker, food);
                            _objManager.NotActiveFoodIndexQueue.Enqueue(_objManager.GetIndexOfObject(obj));
                        }
                    }
                }
            }
        }
    }




    #region CheckExausteFoods related funcs

    void HitFood(RaycastHit2D hitInfo)
    {
        GameObject obj = hitInfo.transform.gameObject;


        float suckRange = (float)_sucker.GetManualSuckerRange();
        float dist = Vector3.Distance(hitInfo.point, _playerObj.transform.position);
        Debug.Log("Hit info: " + hitInfo.point);
        Debug.Log("Dist: " + dist);

        if (dist < suckRange)
        {
            // suck procedure
            // fuel+, tech+, mass+
            Food food = obj.GetComponent<Food>();
            SuckFoodProcess(_sucker, food);

            _objManager.NotActiveFoodIndexQueue.Enqueue(_objManager.GetIndexOfObject(obj));
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
        foreach (var index in _objManager.MovingFoodIndexes)
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
        List<int> indexListOutOfRange = new List<int>();
        foreach (var index in _objManager.MovingFoodIndexes)
        {
            GameObject foodObj = _objManager.FoodPool[index];
            Food food = foodObj.GetComponent<Food>();

            if (food.standardPos.x < _removingPoint.position.x)
            {
                // Debug.Log("Metor OUT!");
                indexListOutOfRange.Add(index);
                continue; 
            }

            float scrollSpeed = _statManager.GetScrollSpeed();
            foodObj.transform.position -= new Vector3(scrollSpeed, 0, 0) * Time.deltaTime;

            float standardScrollSpeed = scrollSpeed * (float)Math.Pow(2d, _statManager.CurrentScaleStep - food.standardScaleStep);
            food.standardPos -= new Vector3(standardScrollSpeed, 0, 0) * Time.deltaTime;
        }

        foreach (var i in indexListOutOfRange)
        {
            GameObject food = _objManager.FoodPool[i];
            food.SetActive(false);
            _objManager.NotActiveFoodIndexQueue.Enqueue(i);
            _objManager.MovingFoodIndexes.Remove(i);
            _autoSuckCheckIndex--;
        }

        indexListOutOfRange.Clear();
    }

    void ApplyCurrentScaleToAllFood(double oldScale, double newScaleStep)
    {
        Player player = _playerObj.GetComponent<Player>();

        //every object has its own scalestep. 
        double standardScaleStep = player.standardScaleStep;
        float newLocalScaleOfPlayer = (float)Math.Pow(2, standardScaleStep - newScaleStep);

        // Debug.Log("===== Oldscale:" + oldScale + " NewScale:" + newScaleStep + "standard:" + standardScaleStep);
        EffectManager.ScaleChange(_playerObj, newLocalScaleOfPlayer);

        foreach( int activeIndex in _objManager.MovingFoodIndexes)
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
