using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Pool<T> where T : Component
{
    public int AvailableObjectsCount => _pool.Count;
    public int Size => _size;

    private Queue<T> _pool = new ();
    private Func<T> _factoryAction; 
    private int _size;

    public Pool(Func<T> factoryAction, int initialSize)
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
        T obj = _factoryAction?.Invoke();
        if (obj == null)
            return;
        
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
        
        _size += 1;
    }

    public T GetObject()
    {
        if (_pool.Count == 0)
        {
            CreateObject();
        }

        var obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void ReturnObject(T obj)
    {
        if (obj == null)
            return;

        _pool.Enqueue(obj);
        obj.gameObject.SetActive(false);
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