using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> FoodPool { get { return _foodObjPool; } }
    public Queue<int> InactiveFoodIndexQueue { get { return _inactiveFoodIndexQueue; } }
    public List<int> ActiveFoodIndexes { get { return _activeFoodIndexes; } }


    [SerializeField] private Transform _xMaxTransform;
    [SerializeField] private Transform _yMaxTransform;
    [SerializeField] private BoxCollider _defaultBox;
    [SerializeField] private StatManager _statManager;
    [SerializeField] private Sprite[] _prefabs;
    [SerializeField] private FoodManager foodManager;
    
    private List<GameObject> _foodObjPool;
    private Queue<int> _inactiveFoodIndexQueue;
    private List<int> _activeFoodIndexes;
    private int _loadingNumOfOnePrefab = 20;
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
        _inactiveFoodIndexQueue = new Queue<int>();
        _activeFoodIndexes = new List<int>();
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

                BoxCollider2D box = foodObj.AddComponent<BoxCollider2D>();

                foodObj.SetActive(false);
                foodObj.name = newFood.name;
                foodObj.transform.tag = "Food";

                _foodObjPool.Add(foodObj);

            }
        }

        //All into inactive queue.
        for (int id = 0; id < _foodObjPool.Count; id++)
        {
            _inactiveFoodIndexQueue.Enqueue(id);
        }
    }

    public void SpawnNewFood(double curScale)
    {
        if (_inactiveFoodIndexQueue.Count > 0)
        {
            Debug.Log("Spawn New Food");
            int newObjectIndex = _inactiveFoodIndexQueue.Dequeue();
            float x = UnityEngine.Random.Range(_xMax, 2 * _xMax);
            float y = UnityEngine.Random.Range(-_yMax, _yMax);

            PlaceFood(x, y, curScale, newObjectIndex);
        }
    }

    void PlaceFood(float x, float y,double curScale, int newObjectIndex)
    {
        GameObject foodObj = _foodObjPool[newObjectIndex];
        
        Food food = foodObj.GetComponent<Food>();
        food.standardPos = new Vector3(x, y, 0);
        Debug.Log("Place Food : " + x + "," + y + " curscale:" + curScale +" newId:" + newObjectIndex);
        foodObj.transform.position = new Vector3(x, y, 0) * (float)Math.Pow(2, food.standardScaleStep - curScale); ;
        foodObj.SetActive(true);

        _activeFoodIndexes.Add(newObjectIndex);
    }

    public Sprite GetSprite(int index)
    {
        return _prefabs[index];
    }

    public int GetIndexOfObject(GameObject obj)
    {
        return _foodObjPool.IndexOf(obj);
    }

    public void LoadInitialLevel(int initiaMaxlLevel)
    {
        Vector3 random;
        float currentXMax = _xMax * Mathf.Pow(2, -initiaMaxlLevel);
        float currentYMax = _yMax * Mathf.Pow(2, -initiaMaxlLevel);
        for (int i = 0; i <= _preLoadingLevelInterval + initiaMaxlLevel; i++)
        {
            for (int j = 0; j < _loadingNumOfOnePrefab; j++)
            {
                int newObjectIndex = i * _loadingNumOfOnePrefab + j;
                
                do
                {
                    random = new Vector3(UnityEngine.Random.Range(-currentXMax, currentXMax), UnityEngine.Random.Range(-currentYMax, currentYMax), 0);
                } while (_defaultBox.bounds.Contains(random));

                PlaceFood(random.x, random.y, _statManager.CurrentScaleStep , newObjectIndex);
            }
            currentXMax *= 2;
            currentYMax *= 2;
            _defaultBox.transform.localScale *= 2;
        }
    }

    public void LoadNewLevelObjects(int loadingLevel)
    {
        int preLoadingLevel = loadingLevel + _preLoadingLevelInterval;
        if (preLoadingLevel < _prefabs.Length)
        {
            for (int i = 0; i < _loadingNumOfOnePrefab; i++)
            {
                int newObjectIndex = preLoadingLevel * _loadingNumOfOnePrefab + i;

                Vector3 random;

                do
                {
                    random = new Vector3(UnityEngine.Random.Range(-_xMax * 2, _xMax * 2), UnityEngine.Random.Range(-_yMax * 2, _yMax * 2), 0);
                } while (_defaultBox.bounds.Contains(random));

                PlaceFood(random.x, random.y, _statManager.CurrentScaleStep, newObjectIndex);
            }
            _defaultBox.transform.localScale *= 2;
        }
    }
}
