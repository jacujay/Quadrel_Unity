﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    /// <summary>
    /// 对象池类，测试使用
    /// </summary>
    public LinkedList<Transform> workingLinkedList;
    public LinkedList<Transform> idleLinkedList;
    [HideInInspector]
    public GameObject prefab;
    private Dictionary<Transform, ObjectPool> poolDictionary;
    /// 单例模式
    private static ObjectPool instance;
	
    /// <summary>
    /// 获取单例对象
    /// </summary>
    public ObjectPool GetInstance()
    {
        if (instance == null)
        {
            instance = new ObjectPool();
        }
        return instance;
    }
    /// <summary>
    /// 初始化对象池
    /// </summary>
    public void InitObjectPool(GameObject prefab,Dictionary<Transform,ObjectPool> poolDictionary,
        int loadNum = 20,Vector3 pos = new Vector3(),Quaternion rotate = new Quaternion())
    {
        this.prefab = prefab;
        this.poolDictionary = poolDictionary;
        workingLinkedList = new LinkedList<Transform>();
        idleLinkedList = new LinkedList<Transform>();
        for(int i = 0; i < loadNum; i++)
        {
            GameObject go = GameObject.Instantiate(prefab, pos, rotate);
            go.SetActive(true);
            go.transform.SetParent(transform);
            idleLinkedList.AddFirst(go.transform);
            poolDictionary.Add(go.transform, this);
        }
    }
    
    public Transform PopObjectFromObjectPool(Vector3 pos = new Vector3(), Quaternion rotate = new Quaternion())
    {
        Transform objectTransform = null;
        //假如对象池已经有对象了
        if (idleLinkedList.Count > 0)
        {
            objectTransform = idleLinkedList.First.Value;
            objectTransform.gameObject.SetActive(true);
            idleLinkedList.RemoveFirst();
            workingLinkedList.AddLast(objectTransform.transform);
        }
        //对象池中没有对象，生成实例
        else
        {
            objectTransform = GameObject.Instantiate(prefab, pos, rotate).transform;
            objectTransform.SetParent(transform);
            workingLinkedList.AddLast(objectTransform.transform);
            poolDictionary.Add(objectTransform, this);
        }
        return objectTransform;
    }

    public void PushObjectToPool(Transform objectTransform,float delayTime = 0.0f)
    {
        StartCoroutine("DelayPushObjectToPool", delayTime);
    }

    IEnumerable DelayPushObjectToPool(Transform objectTransform, float delayTime = 0.0f)
    {
        while (delayTime > 0)
        {
            yield return null;
            if(!objectTransform.gameObject.activeInHierarchy)
            {
                yield break;
            }
            delayTime -= Time.deltaTime;
        }
        if (objectTransform.gameObject.activeSelf)
        {
            objectTransform.gameObject.SetActive(true);
            idleLinkedList.AddFirst(objectTransform);
            workingLinkedList.Remove(objectTransform);
        }
    }
	
}