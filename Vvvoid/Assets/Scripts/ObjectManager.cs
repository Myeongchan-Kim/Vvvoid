using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> FoodPool { get { return _foodObjPool; } }
    public Queue<int> InactiveFoodIndexes { get { return _inactiveFoodIndexes; } }
    public List<int> ActiveFoodIndexes { get { return _activeFoodIndexes; } }

    [SerializeField] private Transform _startXTransform;
    [SerializeField] private Transform _startYTransform;
    [SerializeField] private BoxCollider _visibleArea;
    [SerializeField] private StatManager _statManager;
    [SerializeField] private Sprite[] _prefabs;

    private float _startXPosition;
    private float _startYPosition;
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
        // there is a bug.
    }

    public void MakeObjectPool()
    {
        int scaleFactor = 1;
        for (int i = 0; i < _prefabs.Length; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                GameObject foodObj = new GameObject();

                SpriteRenderer renderer = foodObj.AddComponent<SpriteRenderer>();
                renderer.sprite = _prefabs[i];

                Food newFood = foodObj.AddComponent<Food>();
                newFood.levelToReveal = i;
                newFood.isExhausted = false;

                BoxCollider2D box = foodObj.AddComponent<BoxCollider2D>();

                foodObj.transform.localScale *= scaleFactor;
                foodObj.SetActive(false);

                foodObj.name = "level " + i + " Food";
                foodObj.transform.tag = "Food";

                _foodObjPool.Add(foodObj);

            }
            scaleFactor++;
        }
    }
    
    public void MakeNewLevelChange()
    {
        if (_statManager.maxScaleStep == 0)
        {
            for (int i = 0; i <= _preLoadingLevelInterval; i++)
            {
                for (int j = 0; j < _prefabMaxLoadCount; j++)
                {

                    int newObjectIndex = i * _prefabMaxLoadCount + j;
                    GameObject meteor = _foodObjPool[newObjectIndex];
                    meteor.SetActive(true);
                    meteor.transform.position = new Vector3(Random.Range(-_startXPosition * 2, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                    _activeFoodIndexes.Add(newObjectIndex);


                }
            }
        }
        else
        {
            if (_statManager.maxScaleStep < _prefabs.Length)
            {
                for (int i = 0; i <= _prefabMaxLoadCount; i++)
                { 
                    int newObjectIndex = (int)_statManager.maxScaleStep * _prefabMaxLoadCount + i;
                    GameObject meteor = _foodObjPool[newObjectIndex];
                    meteor.SetActive(true);
                    Vector3 random;
                    do
                    {
                        random = new Vector3(Random.Range(-_startXPosition * 2, _startXPosition * 2), Random.Range(-_startYPosition * 2, _startYPosition * 2), 0);
                    } while (_visibleArea.bounds.Contains(random));
                    meteor.transform.position = random;
                    _activeFoodIndexes.Add(newObjectIndex);
                }
                //                 List<int> indexes = new List<int>();
                //                 foreach (var index in _inactiveFoodIndexes)
                //                 {
                //                     indexes.Add(index);
                //                 }
                //                 _inactiveFoodIndexes.Clear();
                // 
                //                 for (int i = _prefabMaxLoadCount * (int)_statManager.maxScaleStep + 1; i <= _prefabMaxLoadCount * ((int)_statManager.maxScaleStep + 1); i++)
                //                 {
                //                     indexes.Add(i);
                //                 }
                // 
                //                 indexes.Sort((x, y) => Random.value < 0.5f ? -1 : 1);
                // 
                //                 for (int i = 0; i < _prefabMaxLoadCount * (_statManager.maxScaleStep + 1); i++)
                //                 {
                //                     _inactiveFoodIndexes.Enqueue(indexes[i]);
                //                 }
            }

            
        }
    }

    public int GetIndexOfObject(GameObject obj)
    {
        return _foodObjPool.IndexOf(obj);
    }

//     public void LoadNewLevelObjects(int currentLoadingLevel)
//     {
//         if (currentLoadingLevel < _prefabs.Length)
//         {
//             List<int> indexes = new List<int>();
//             foreach (var index in _inactiveFoodIndexes)
//             {
//                 indexes.Add(index);
//             }
//             _inactiveFoodIndexes.Clear();
// 
//             for (int i = _prefabMaxLoadCount * currentLoadingLevel + 1; i <= _prefabMaxLoadCount * (currentLoadingLevel + 1); i++)
//             {
//                 indexes.Add(i);
//             }
// 
//             indexes.Sort((x, y) => Random.value < 0.5f ? -1 : 1);
// 
//             for (int i = 0; i < _prefabMaxLoadCount * (_statManager.maxScaleStep + 1); i++)
//             {
//                 _inactiveFoodIndexes.Enqueue(indexes[i]);
//             }
//         }
//     }
}
