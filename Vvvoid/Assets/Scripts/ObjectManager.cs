using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> FoodPool { get { return _foodObjPool; } }
    public Queue<int> InactiveFoodIndexQueue { get { return _inactiveFoodIndexQueue; } }
    public List<int> ActiveFoodIndexes { get { return _activeFoodIndexes; } }
    // 
    //     [SerializeField] private Transform _startXTransform;
    //     [SerializeField] private Transform _startYTransform;
    [SerializeField]
    private Transform _xMaxTransform;
    [SerializeField]
    private Transform _yMaxTransform;
    [SerializeField] private BoxCollider _visibleArea;
    [SerializeField] private StatManager _statManager;
    [SerializeField] private Sprite[] _prefabs;
// 
//     private float _startXPosition;
//     private float _startYPosition;
    [SerializeField]
    private FoodManager foodManager;
    
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
                newFood = foodManager.FillFoodInfoByIndex(i, newFood);

                SpriteRenderer renderer = foodObj.AddComponent<SpriteRenderer>();
                renderer.sprite = newFood.spirte;

                BoxCollider2D box = foodObj.AddComponent<BoxCollider2D>();

                foodObj.SetActive(false);
                foodObj.name = newFood.name;
                foodObj.transform.tag = "Food";

                _foodObjPool.Add(foodObj);

            }
        }
    }

    public void SpawnNewFood()
    {
        if (_inactiveFoodIndexQueue.Count > 0)
        {
            int newObjectIndex = _inactiveFoodIndexQueue.Dequeue();
            float x = UnityEngine.Random.Range(-_xMax, _xMax);
            float y = UnityEngine.Random.Range(-_yMax, _yMax);
            PlaceFood(x, y, newObjectIndex);
        }
    }

    void PlaceFood(float x, float y, int newObjectIndex)
    {
        GameObject foodObj = _foodObjPool[newObjectIndex];
        foodObj.SetActive(true);

        Food food = foodObj.GetComponent<Food>();
        food.standardPos = new Vector3(x, y, 0);
        foodObj.transform.position = new Vector3(x, y, 0);

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

    public void LoadNewLevelObjects(int loadingLevel)
    {
        if (loadingLevel == 0)
        {
            for (int i = 0; i <= _preLoadingLevelInterval; i++)
            {
                for (int j = 0; j < _loadingNumOfOnePrefab; j++)
                {

                    int newObjectIndex = i * _loadingNumOfOnePrefab + j;

                    float x = Random.Range(-_xMax * 2, _xMax * 2);
                    float y = Random.Range(-_yMax * 2, _yMax * 2);
                    
                    PlaceFood(x, y, newObjectIndex);
                }
            }
        }
        else
        {
            int preLoadingLevel = loadingLevel + _preLoadingLevelInterval;
            if (preLoadingLevel < _prefabs.Length)
            {
                for (int i = 0; i <= _loadingNumOfOnePrefab; i++)
                { 
                    int newObjectIndex = preLoadingLevel * _loadingNumOfOnePrefab + i;
                    
                    Vector3 random;
                    do
                    {
                        random = new Vector3(Random.Range(-_xMax * 2, _xMax * 2), Random.Range(-_yMax * 2, _yMax * 2), 0);
                    } while (_visibleArea.bounds.Contains(random));
                    
                    PlaceFood(random.x, random.y, newObjectIndex);
                }
            }

            
        }
    }
    
}
