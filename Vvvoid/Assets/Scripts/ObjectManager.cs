using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> foodPool { get { return _foodPool; } }
    public Queue<int> InactiveFoodIndexes { get { return _inactiveFoodIndexes; } }
    public List<int> ActiveFoodIndexes { get { return _activeFoodIndexes; } }

    [SerializeField] private StatManager _statManager;
    [SerializeField] private Sprite[] _prefabs;
    
    private List<GameObject> _foodPool;
    private Queue<int> _inactiveFoodIndexes;
    private List<int> _activeFoodIndexes;
    private int _prefabMaxLoadCount = 20;
    private int _preLoadingLevelInterval = 1;

    void Start()
    {
        _foodPool = new List<GameObject>();
        _inactiveFoodIndexes = new Queue<int>();
        _activeFoodIndexes = new List<int>();
    }

    public void MakeObjects()
    {
        int scaleFactor = 1;
        for (int i = 0; i < _prefabs.Length; i++)
        {
            for (int j = 0; j < _prefabMaxLoadCount; j++)
            {
                GameObject obj = new GameObject();
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = _prefabs[i];
                Food newFood = obj.AddComponent<Food>();
                newFood.levelToReveal = i;
                newFood.isExhausted = false;
                BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

                obj.transform.localScale *= scaleFactor;
                obj.SetActive(false);
                _foodPool.Add(obj);
                obj.name = "level " + i + " Food";
                obj.transform.tag = "Food";
            }
            scaleFactor++;
        }
    }

    public int GetIndexOfObject(GameObject obj)
    {
        return _foodPool.IndexOf(obj);
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
