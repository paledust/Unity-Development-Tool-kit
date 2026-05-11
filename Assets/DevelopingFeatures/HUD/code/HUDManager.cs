using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ObjectPool;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HUD
{
    public enum HUDPriority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    public class HUDManager : Singleton<HUDManager>
    {
        [SerializeField] private Transform lowPriorityParent;
        [SerializeField] private Transform normalPriorityParent;
        [SerializeField] private Transform highPriorityParent;

    [Header("Text Pop Group")]
        [SerializeField] private GameObject hudTextPrefab;
        [SerializeField, Range(0, 1)] private float textAnimationDurationMulti = 0.5f;
    [Header("Progress Bar Group")]
        [SerializeField] private AssetReferenceGameObject hudProgressBarPrefab;

        private Dictionary<Type, IHUDComponent> dictHUDComponents;
        private HashSet<string> poolHash = new HashSet<string>();

        protected override void Awake()
        {
            base.Awake();
            dictHUDComponents = new Dictionary<Type, IHUDComponent>();

            var hudComponents = GetComponents<IHUDComponent>();
            foreach (var component in hudComponents)
            {
                var type = component.GetType();
                if(dictHUDComponents.ContainsKey(type))
                {
                    Debug.LogError($"Duplicate HUD Component Type Detected : {type}");
                    continue;
                }
                dictHUDComponents.Add(type, component);
                component.Init();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var component in dictHUDComponents.Values)
                component.CleanUp();
            //Clean Up Pools
            if(PoolManager.Instance == null)
                return;
            foreach (var poolName in poolHash)
                PoolManager.Instance.Dispose(poolName);
        }
        private Transform GetParentByPriority(HUDPriority priority)
        {
            return priority switch
            {
                HUDPriority.Low => lowPriorityParent,
                HUDPriority.Normal => normalPriorityParent,
                HUDPriority.High => highPriorityParent,
                _ => normalPriorityParent
            };
        }
        private Vector2 GetScreenPos(Vector3 worldPos) => Camera.main.WorldToScreenPoint(worldPos);
        public T PopHUDElementAtWorld<T>(Vector2 pos, HUDPriority priority, GameObject prefab, bool poolOption = false) where T : IHUD
            => PopHUDElement<T>(GetScreenPos(pos), priority, prefab, poolOption);
        public T PopHUDElement<T>(Vector2 pos, HUDPriority priority, GameObject prefab, bool poolOption) where T : IHUD
        {
            Transform parent = GetParentByPriority(priority);
            GameObject uiObj;

            if(poolOption)
            {
                uiObj = PoolManager.Instance.GetObject(prefab.name, prefab, parent);
                if (!poolHash.Contains(prefab.name))
                {
                    poolHash.Add(prefab.name);
                }
            }
            else
                uiObj = Instantiate(prefab, parent);
            

            uiObj.transform.position = pos;
            uiObj.transform.SetAsLastSibling();
            return uiObj.GetComponent<T>();
        }
        static void ClearHUDPool(string poolName, GameObject go) 
            => PoolManager.Instance.Release(poolName, go);

#region HUD Component
        public T GetHUDComponent<T>() where T : class, IHUDComponent
        {
            var type = typeof(T);
            if (dictHUDComponents.TryGetValue(type, out var component))
                return component as T;
            Debug.LogError($"HUD Component of type {type} not found!");
            return default;
        }
#endregion

#region HUD Text
        public void PopTextAtWorld(Vector3 worldPos, TextDisplayData displayData, HUDPriority priority = HUDPriority.Low) 
        {
            var hud_text = PopHUDElementAtWorld<HUD_Text>(worldPos, priority, hudTextPrefab, true);
            hud_text.Init(displayData, textAnimationDurationMulti, () => ClearHUDPool(hudTextPrefab.name, hud_text.gameObject));
        }
#endregion
    }
}