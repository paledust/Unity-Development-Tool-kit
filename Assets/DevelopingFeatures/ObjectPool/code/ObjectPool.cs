using System.Collections.Generic;
using System;

namespace ObjectPool
{
    public class ObjectPool<T> where T : class
    {
        public int MaxPoolSize { get; private set; }        // 对象池的最大大小

        private readonly Stack<T> pool;                 // 对象池
        private readonly HashSet<T> activeObjects;         // 启用标记
        private readonly Func<T> onCreateObject;          // 创建对象的方法
        private readonly Action<T> onTakeObject;     // 提取对象的方法
        private readonly Action<T> onRecycleObject;       // 回收对象的方法
        private readonly Action<T> onDestroyObject;       // 销毁对象的方法
        private int currentSize;                        // 当前对象池中对象的数量
        public int m_currentSize => currentSize;
        public int m_activeSize => activeObjects.Count;

        // 构造函数，设置最大大小、创建对象的方法、提取对象的方法、回收对象的方法、销毁对象的方法
        public ObjectPool(int maxSize, Func<T> onCreateObject, Action<T> onTakeObject, Action<T> onRecycleObject, Action<T> onDestroyObject)
        {
            MaxPoolSize = maxSize;
            this.onCreateObject = onCreateObject;
            this.onRecycleObject = onRecycleObject;
            this.onDestroyObject = onDestroyObject;
            this.onTakeObject = onTakeObject;
            activeObjects = new HashSet<T>();
            pool = new Stack<T>();
        }

        // 从对象池中获取对象
        public T TakeObject()
        {
            T obj;
            if (pool.Count > 0)
            {
                obj = pool.Pop();
            }
            else
            {
                obj = onCreateObject();
                currentSize++;
            }
            activeObjects.Add(obj);
            onTakeObject?.Invoke(obj);
            return obj;
        }

        // 回收对象到对象池中，如果对象池已经达到最大容量，则直接销毁对象
        public void RecycleObject(T obj)
        {
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
                onRecycleObject?.Invoke(obj);
                if (pool.Count < MaxPoolSize)
                {
                    pool.Push(obj);
                }
                else
                {
                    onDestroyObject?.Invoke(obj);
                    currentSize--;
                }
            }
        }

        // 回收所有已经启用的对象到对象池中
        public void RecycleAllObjects()
        {
            foreach (T obj in activeObjects)
            {
                if (pool.Count < MaxPoolSize)
                {
                    onRecycleObject?.Invoke(obj);
                    pool.Push(obj);
                }
                else
                {
                    onDestroyObject?.Invoke(obj);
                }
            }

            activeObjects.Clear();
        }

        //销毁池
        public void DestroyPool()
        {
            while (pool.Count > 0)
            {
                T obj = pool.Pop();
                onDestroyObject?.Invoke(obj);
            }
            pool.Clear();
            foreach (T obj in activeObjects)
            {
                onDestroyObject?.Invoke(obj);
            }
            activeObjects.Clear();
        }
    }
}