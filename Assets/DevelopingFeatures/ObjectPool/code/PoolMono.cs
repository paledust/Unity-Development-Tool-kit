using System;
using UnityEngine;

namespace ObjectPool
{
    public class PoolMono : MonoBehaviour
    {
        [SerializeField] private int poolMaxSize;
        [SerializeField] private PoolReleaseMode releaseMode;
        [SerializeField] private Transform poolParent;

        private GameObject poolObject;
        private ObjectPool<GameObject> mPool;
        private Action<GameObject> onRelease;

        public void PoolInit(GameObject poolObject, int poolMaxSize, Transform poolParent, PoolReleaseMode releaseMode, Action<GameObject> onRelease)
        {
            if (mPool != null)
            {
                mPool.DestroyPool();
            }
            this.poolParent = poolParent;
            this.poolObject = poolObject;
            this.poolMaxSize = poolMaxSize;
            this.releaseMode = releaseMode;
            this.onRelease = onRelease;
            mPool = new ObjectPool<GameObject>(poolMaxSize, OnCreatePoolItem, OnGetPoolItem, OnRelesePoolItem, OnDestroyPoolItem);
        }
        public GameObject Get()
        {
            if (mPool == null)
            {
                mPool = new ObjectPool<GameObject>(poolMaxSize, OnCreatePoolItem, OnGetPoolItem, OnRelesePoolItem, OnDestroyPoolItem);
            }
            GameObject are = mPool.TakeObject();
            return are;
        }
        public void Release(GameObject go)
        {
            mPool.RecycleObject(go);
        }
        public void ReleaseAll()
        {
            mPool.RecycleAllObjects();
        }
        public void Dispose()
        {
            mPool.DestroyPool();
        }

        void OnDestroyPoolItem(GameObject obj)
        {
            Destroy(obj.gameObject);
        }
        void OnRelesePoolItem(GameObject go)
        {
            switch (releaseMode)
            {

                case PoolReleaseMode.Normal:
                    break;

                case PoolReleaseMode.AllChild:
                    foreach (Transform t in go.transform)
                        Destroy(t.gameObject);

                    break;

                case PoolReleaseMode.AllComp:
                    foreach (Component comp in go.GetComponents<Component>())
                    {
                        if (comp is Transform || comp is SpriteRenderer) continue;
                        Destroy(comp);
                    }
                    break;

                case PoolReleaseMode.AllChildAndComp:
                    foreach (Component comp in go.GetComponents<Component>())
                    {
                        if (comp is Transform || comp is SpriteRenderer) continue;
                        Destroy(comp);
                    }
                    foreach (Transform t in go.transform)
                        Destroy(t.gameObject);
                    break;

                case PoolReleaseMode.Other:
                    if (onRelease != null)
                    {
                        onRelease(go);
                    }
                    return;
                case PoolReleaseMode.SetParent:
                    go.transform.SetParent(poolParent);
                    break;
            }
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.gameObject.SetActive(false);
            go.name = PoolManager.POOL_KEYWORD;
        }
        void OnGetPoolItem(GameObject obj)
        {
            obj.gameObject.SetActive(true);
        }
        GameObject OnCreatePoolItem()
        {
            var obj = Instantiate(poolObject, Vector2.zero, Quaternion.identity, poolParent);
            return obj;
        }

        public int GetCurrentSize() => mPool.m_currentSize;
        public int GetActiveSize() => mPool.m_activeSize;
    }
}