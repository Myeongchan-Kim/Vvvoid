using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> foodPool { get { return _foodObjPool; } }
    public Queue<int> InactiveFoodIndexes { get { return _inactiveFoodIndexes; } }
    public List<int> ActiveFoodIndexes { get { return _activeFoodIndexes; } }

    [SerializeField] private StatManager _statManager;
    [SerializeField] private Sprite[] _prefabs;
    [SerializeField]
    private FoodManager foodManager;
    
    private List<GameObject> _foodObjPool;
    private Queue<int> _inactiveFoodIndexes;
    private List<int> _activeFoodIndexes;
    private int _prefabMaxLoadCount = 20;
    private int _preLoadingLevelInterval = 1;

    void OnEnable()
    {
        _foodObjPool = new List<GameObject>();
        _inactiveFoodIndexes = new Queue<int>();
        _activeFoodIndexes = new List<int>();
    }

    void Start()
    {
        // there is a bug that caused by using _foodObjPool before init. So I move this code to OnEnable(). 
    }

    public void MakeObjects()
    {
        for (int i = 0; i < foodManager.foodDatas.Length; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {

                GameObject foodObj = new GameObject();

                Food newFood = foodObj.AddComponent<Food>();
                newFood = foodManager.FillFoodInfoByIndex(i, newFood);

                SpriteRenderer renderer = foodObj.AddComponent<SpriteRenderer>();
                renderer.sprite = newFood.spirte;

                BoxCollider2D box = foodObj.AddComponent<BoxCollider2D>();

                //foodObj.transform.localScale *= scaleFactor;
                foodObj.SetActive(false);

                foodObj.name = newFood.name;
                foodObj.transform.tag = "Food";

                _foodObjPool.Add(foodObj);

            }
        }
    }

    public void PlaceFood(float startXPos, float startYPos, int newObjectIndex)
    {
        GameObject foodObj = foodPool[newObjectIndex];
        foodObj.SetActive(true);

        Food food = foodObj.GetComponent<Food>();
        food.standardPos = new Vector3(UnityEngine.Random.Range(-startXPos, startXPos), UnityEngine.Random.Range(-startYPos, startYPos), 0);

        ActiveFoodIndexes.Add(newObjectIndex);
    }

    public Sprite GetSprite(int index)
    {
        return _prefabs[index];
    }

    public int GetIndexOfObject(GameObject obj)
    {
        return _foodObjPool.IndexOf(obj);
    }

    public void LoadNewLevelObjects(int currentLoadingLevel)
    {
        if (currentLoadingLevel < _prefabs.Length)
        {
            List<int> indexes = new List<int>();
            foreach (var index in _inactiveFoodIndexes)
            {
                indexes.Add(index);
            }
            _inactiveFoodIndexes.Clear();

            for (int i = _prefabMaxLoadCount * currentLoadingLevel + 1; i <= _prefabMaxLoadCount * (currentLoadingLevel + 1); i++)
            {
                indexes.Add(i);
            }

            indexes.Sort((x, y) => Random.value < 0.5f ? -1 : 1);

            for (int i = 0; i < _prefabMaxLoadCount * (_statManager.maxScaleStep + 1); i++)
            {
                _inactiveFoodIndexes.Enqueue(indexes[i]);
            }
        }
    }
}
