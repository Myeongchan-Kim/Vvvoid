using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> FoodPool { get { return _foodObjPool; } }
    public Queue<int> NotActiveFoodIndexQueue { get { return _notMovingFoodIndexQueue; } }
    public List<int> MovingFoodIndexes { get { return _movingFoodIndexes; } }


    [SerializeField] private Transform _xMaxTransform;
    [SerializeField] private Transform _yMaxTransform;
    [SerializeField] private BoxCollider _defaultBox;
    [SerializeField] private StatManager _statManager;
    [SerializeField] private FoodManager foodManager;
    
    private List<GameObject> _foodObjPool;
    private Queue<int> _notMovingFoodIndexQueue;
    private List<int> _movingFoodIndexes;
    private int _loadingNumOfOnePrefab = 40;
    private int _preLoadingLevelInterval;
    private float _xMax;
    private float _yMax;

    public void SetloadingNumOfOnePrefab(int loadCount)
    {
        _loadingNumOfOnePrefab = loadCount;
    }

    public void SetPreLoadingLevelInterval(int interval)
    {
        _preLoadingLevelInterval = interval;
    }

    void OnEnable()
    {
        _foodObjPool = new List<GameObject>();
        _notMovingFoodIndexQueue = new Queue<int>();
        _movingFoodIndexes = new List<int>();
        _xMax = _xMaxTransform.position.x;
        _yMax = _yMaxTransform.position.y;
    }

    void Start()
    {
        // there is a bug that caused by using _foodObjPool before init. So I move this code to OnEnable(). 
    }

    public void MakeObjectPool()
    {
        for (int i = 0; i < foodManager.foodDatas.Length; i++)
        {
            for (int j = 0; j < _loadingNumOfOnePrefab; j++)
            {

                GameObject foodObj = new GameObject();

                Food newFood = foodObj.AddComponent<Food>();
                newFood = foodManager.FillFoodInfoByLevel(i, newFood);

                SpriteRenderer renderer = foodObj.AddComponent<SpriteRenderer>();
                renderer.sprite = newFood.sprite;

                foodObj.AddComponent<BoxCollider2D>();

                foodObj.SetActive(false);
                foodObj.name = newFood.name;
                foodObj.transform.tag = "Food";

                _foodObjPool.Add(foodObj);

            }
        }
        
        //shuffle List
        List<int> shuffleList = new List<int>();
        shuffleList.Capacity = _foodObjPool.Count;
        for (int id = 0; id < _foodObjPool.Count; id++)
        {
            int ranid = UnityEngine.Random.Range(0, id);
            shuffleList.Insert(ranid, id);
        }
        
        for (int i = 0; i < _foodObjPool.Count; i++)
        {
            _notMovingFoodIndexQueue.Enqueue(shuffleList[i]);
        }
    }

    public bool SpawnNewFood(double curScale)
    {
        bool isFoodSpawned = false;

        while (isFoodSpawned == false && _notMovingFoodIndexQueue.Count > 0)
        {
            int newObjectIndex = _notMovingFoodIndexQueue.Dequeue();

            //float x = UnityEngine.Random.Range(_xMax, 2 * _xMax);
            float x = _xMax;
            float y = UnityEngine.Random.Range(-_yMax, _yMax);
            isFoodSpawned = PlaceFood(x, y, curScale, newObjectIndex);

            if ( isFoodSpawned == false)
            {
                // If food doesn't fit cur level, 
                _notMovingFoodIndexQueue.Enqueue(newObjectIndex);
            }
            else
            {
                isFoodSpawned = true;
                break;
            }
        }
        return isFoodSpawned;
    }

    bool PlaceFood(float x, float y, double curScale, int newObjectIndex)
    {
        GameObject foodObj = _foodObjPool[newObjectIndex];
        
        Food food = foodObj.GetComponent<Food>();
        //Check food is fit current scale.
        if (curScale > food.maxScaleStep || curScale < food.minScaleStep)
            return false;

        Debug.Log("Place Food : " + x + "," + y + 
            " curscale:" + curScale +
            " newId:" + newObjectIndex + 
            " min:" + food.minScaleStep + " max:" + food.maxScaleStep);

        food.standardPos = new Vector3(x, y, 0);
        foodObj.transform.position = new Vector3(x, y, 0) * (float)Math.Pow(2, food.standardScaleStep - curScale);

        float newLocalScaleOfFood = (float)Math.Pow(2, food.standardScaleStep - curScale);
        foodObj.transform.localScale = new Vector3(1, 1, 1) * newLocalScaleOfFood;
       
        foodObj.SetActive(true);
        
        _movingFoodIndexes.Add(newObjectIndex);

        return true;
    }

    public int GetIndexOfObject(GameObject obj)
    {
        return _foodObjPool.IndexOf(obj);
    }
}
