using UnityEngine;

namespace BattleMap.Grid
{
    public interface IGridNode
    {
        public Vector2Int gridXY { get; }
        public Vector2 worldPos { get; }
    }
}
