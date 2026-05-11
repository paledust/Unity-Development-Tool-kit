using System.Collections.Generic;
using UnityEngine;
using System;

namespace ObjectPool
{
    public enum PoolReleaseMode
    {
        Normal,
        AllComp,
        AllChild,
        AllChildAndComp,
        SetParent,
        Other,
    }
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, PoolMono> poolGroup = new Dictionary<string, PoolMono>();
        private Transform parent;
        internal const string POOL_KEYWORD = "n_pool";

        public GameObject GetObject(string poolName, GameObject prefabObject)
        {
            if (!poolGroup.ContainsKey(poolName))
            {
                CreatePoolObject(poolName, prefabObject);
            }
            return poolGroup[poolName].Get();
        }

        public GameObject GetObject(string poolName, GameObject prefabObject, Transform parentGroup = null, bool isUISpace = false, int maxNum = 200, PoolReleaseMode poolRelease = PoolReleaseMode.Normal, Action<GameObject> action = null)
        {
            if (!poolGroup.ContainsKey(poolName))
            {
                CreatePoolObject(poolName, prefabObject, parentGroup, isUISpace, maxNum, poolRelease, action);
            }
            return poolGroup[poolName].Get();
        }
        public int PoolCurrentSize(string poolName)
        {
            if (poolGroup.ContainsKey(poolName))
            {
                return poolGroup[poolName].GetCurrentSize();
            }
            return 0;
        }
        public int PoolActiveSize(string poolName)
        {
            if (poolGroup.ContainsKey(poolName))
            {
                return poolGroup[poolName].GetActiveSize();
            }
            return 0;
        }
        /// <summary>
        /// 标准回收，没有额外组件或子物体。[地块,雾气]
        /// </summary>
        public void Release(string poolName, GameObject go)
        {
            if (poolGroup.ContainsKey(poolName))
                poolGroup[poolName].Release(go);
        }
        /// <summary>
        /// 移除物体池。
        /// </summary>
        public void Dispose(string poolName)
        {
            if (poolGroup.ContainsKey(poolName))
            {
                var pool = poolGroup[poolName];
                pool.Dispose();
                poolGroup.Remove(poolName);
            }
        }
        public void ReleaseAll(string poolName)
        {
            if (poolGroup.ContainsKey(poolName))
                poolGroup[poolName].ReleaseAll();
        }

        /// <summary>
        /// UI层创建需要传入parentGroup, 否则在世界坐标与ui坐标对不上
        /// </summary>
        void CreatePoolObject(string poolName, GameObject prefabObject, Transform parentGroup = null, bool isUISpace = false, int maxNum = 200, PoolReleaseMode poolRelease = PoolReleaseMode.Normal, Action<GameObject> action = null)
        {
            GameObject poolCollectObj = new GameObject(poolName);
            if (parent == null)
                parent = new GameObject("[Pool Root]").transform;
            poolCollectObj.transform.SetParent(parentGroup);
            if (isUISpace)
            {
                poolCollectObj.AddComponent<RectTransform>();
                poolCollectObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            }
            poolCollectObj.transform.localScale = Vector3.one;
            PoolMono poolObject = poolCollectObj.gameObject.AddComponent<PoolMono>();
            poolObject.PoolInit(prefabObject, maxNum, poolCollectObj.transform, poolRelease, action);
            poolGroup.Add(poolName, poolObject);
        }
        public static bool IsCycledInPooled(GameObject gameObject) => gameObject.name == POOL_KEYWORD;
    }
}