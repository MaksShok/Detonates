using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Pool
{
    public int AvailableObjectsCount => _pool.Count;
    public int Size => _size;

    private Queue<GameObject> _pool = new Queue<GameObject>();
    private Func<GameObject> _factoryAction; 
    private int _size;

    public Pool(Func<GameObject> factoryAction, int initialSize)
    {
        _factoryAction = factoryAction;

        InitializePool(initialSize);
    }

    private void InitializePool(int initializeSize)
    {
        for (int i = 0; i < initializeSize; i++)
        {
            CreateObject();
        }
    }

    public void CreateObject()
    {
        GameObject obj = _factoryAction?.Invoke();
        if (obj == null)
            return;
        
        obj.SetActive(false);
        _pool.Enqueue(obj);
        
        _size += 1;
    }

    public GameObject GetObject()
    {
        if (_pool.Count == 0)
        {
            CreateObject();
        }

        var obj = _pool.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null)
            return;

        _pool.Enqueue(obj);
        obj.SetActive(false);
    }

    public void Clear()
    {
        while (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }

        _size = 0;
    }
}