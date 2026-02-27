using System.Collections.Generic;
using UnityEngine;

public class EnemyPool
{
    public int AvailableObjectsCount => _pool.Count;
    public int Size => _size;
    
    private Queue<GameObject> _pool = new Queue<GameObject>();
    private GameObject _prefab;
    private Transform _сontainer;
    private int _size;
    private int _expansionSize;
    
    public EnemyPool(GameObject prefab, int initialSize, int expansionSize = 10, Transform container = null)
    {
        _prefab = prefab;
        _expansionSize = expansionSize;
        _сontainer = container ?? CreateDefaultContainer();

        InitializePool(initialSize);
    }

    private void InitializePool(int initializeSize)
    {
        for (int i = 0; i < initializeSize; i++)
        {
            GameObject obj = GameObject.Instantiate(_prefab, _сontainer);
            obj.name = $"{_prefab.name}_Pooled_{i}";
            obj.SetActive(false);

            _pool.Enqueue(obj);
        }

        _size = initializeSize;
        Debug.Log($"Пул '{_prefab.name}' создан: {initializeSize} объектов");
    }

    public GameObject GetObject()
    {
        if (_pool.Count == 0)
        {
            ExpandPool(_expansionSize);
        }

        GameObject obj = _pool.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null) 
            return;
        
        obj.transform.SetParent(_сontainer);
        obj.SetActive(false);

        _pool.Enqueue(obj);
    }

    public void ExpandPool(int count)
    {
        Debug.Log($"Расширение пула '{_prefab.name}': +{count} объектов (было: {_pool.Count})");

        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(_prefab, _сontainer);
            obj.name = $"{_prefab.name}_Pooled_{_pool.Count}";
            obj.SetActive(false);

            _pool.Enqueue(obj);
        }

        _size += count;
        Debug.Log($"Пул '{_prefab.name}' расширен. Теперь: {_pool.Count} объектов");
    }

    public void Clear()
    {
        while (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }

        _size = 0;
    }

    private Transform CreateDefaultContainer()
    {
        GameObject containerObj = new GameObject($"Pool_{_prefab.name}");
        containerObj.hideFlags = HideFlags.NotEditable;
        return containerObj.transform;
    }
}