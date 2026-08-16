using UnityEngine;
using System.Collections.Generic;

public abstract class DataCollection<T> : ScriptableObject where T: ScriptableObject
{
    [SerializeField] protected List<T> DataList;
    public abstract T GetDataByKey(string key);
    public List<T> GetDataCollection() => DataList;
}