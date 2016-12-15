using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> ResourcePool { get { return _resourcePool; } }
    public Queue<int> InactiveResourceIndexes { get { return _inactiveResourceIndexes; } }
    public List<int> ActiveResourceIndexes { get { return _activeResourceIndexes; } }

    [SerializeField] private StatManager _statManager;
    [SerializeField] private Sprite[] _prefabs;
    
    private List<GameObject> _resourcePool;
    private Queue<int> _inactiveResourceIndexes;
    private List<int> _activeResourceIndexes;
    private int _prefabMaxLoadCount = 20;
    private int _preLoadingLevelInterval = 1;

    void Start()
    {
        _resourcePool = new List<GameObject>();
        _inactiveResourceIndexes = new Queue<int>();
        _activeResourceIndexes = new List<int>();
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
                Resource resource = obj.AddComponent<Resource>();
                resource.levelToReveal = i;
                resource.isExhausted = false;
                BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

                obj.transform.localScale *= scaleFactor;
                obj.SetActive(false);
                _resourcePool.Add(obj);
                obj.name = "level " + i + " resource";
                obj.transform.tag = "Resource";
            }
            scaleFactor++;
        }
    }

    public int GetIndexOfObject(GameObject obj)
    {
        return _resourcePool.IndexOf(obj);
    }

    public void LoadNewLevelObjects(int currentLoadingLevel)
    {
        if (currentLoadingLevel < _prefabs.Length)
        {
            List<int> indexes = new List<int>();
            foreach (var index in _inactiveResourceIndexes)
            {
                indexes.Add(index);
            }
            _inactiveResourceIndexes.Clear();

            for (int i = _prefabMaxLoadCount * currentLoadingLevel + 1; i <= _prefabMaxLoadCount * (currentLoadingLevel + 1); i++)
            {
                indexes.Add(i);
            }

            indexes.Sort((x, y) => Random.value < 0.5f ? -1 : 1);

            for (int i = 0; i < _prefabMaxLoadCount * (_statManager.maxScaleStep + 1); i++)
            {
                _inactiveResourceIndexes.Enqueue(indexes[i]);
            }
        }
    }
}
