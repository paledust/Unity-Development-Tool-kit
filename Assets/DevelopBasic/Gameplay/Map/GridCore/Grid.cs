using System.Collections.Generic;
using UnityEngine;

namespace BattleMap.Grid
{
    [System.Serializable]
    public class GridGraph<T> where T : IGridNode
    {
        [Header("Grid Settings")]
        [SerializeField] protected float nodeWidth = 1;
        [SerializeField] protected Vector2Int gridOffset;
        [SerializeField] protected Vector2Int gridRange;

        protected T[,] nodes;
        public float m_gridWidth => nodeWidth;
        public bool m_hasNodes => nodes != null;

        //构造函数
        public GridGraph(float nodeWidth, Vector2Int gridXY, int offsetX, int offsetY)
        {
            this.nodeWidth = nodeWidth;
            this.gridOffset = new Vector2Int(offsetX, offsetY);
            this.gridRange = gridXY;
            nodes = new T[gridXY.x, gridXY.y];
        }
        //字典更新网格图
        protected internal void UpdateNode(Dictionary<Vector2Int, T> nodeDict)
        {
            foreach (var go in nodeDict)
            {
                nodes[go.Key.x, go.Key.y] = go.Value;
            }
            OnNodeUpdated(nodeDict);
        }
        //当网格图节点数据更新时执行
        protected internal virtual void OnNodeUpdated(Dictionary<Vector2Int, T> nodeDict) { }

        #region GridNode
        internal T GetNode(Vector2Int gridPoint)
        {
            gridPoint.x = Mathf.Clamp(gridPoint.x, 0, gridRange.x - 1);
            gridPoint.y = Mathf.Clamp(gridPoint.y, 0, gridRange.y - 1);
            return nodes[gridPoint.x, gridPoint.y];
        }
        internal T GetNode(Vector2 worldPos)
        {
            Vector2Int gridPoint = GetGridPointFromWorld(worldPos);
            return GetNode(gridPoint);
        }
        internal bool IsPointInGrid(Vector2Int gridPoint)
        {
            return gridPoint.x >= 0 && gridPoint.x < gridRange.x && gridPoint.y >= 0 && gridPoint.y < gridRange.y;
        }
        #endregion

        #region GridPoint
        internal Vector2Int GetGridPointFromWorld(Vector2 worldPos)
        {
            int x = Mathf.RoundToInt((worldPos.x - gridOffset.x) / nodeWidth);
            int y = Mathf.RoundToInt((worldPos.y - gridOffset.y) / nodeWidth);
            x = Mathf.Clamp(x, 0, gridRange.x-1);
            y = Mathf.Clamp(y, 0, gridRange.y-1);

            return new Vector2Int(x, y);
        }
        internal Vector2 GetWorldPosFromGrid(Vector2Int gridPoint)
        {
            gridPoint.x = Mathf.Clamp(gridPoint.x, 0, gridRange.x-1);
            gridPoint.y = Mathf.Clamp(gridPoint.y, 0, gridRange.y-1);
            return nodes[gridPoint.x, gridPoint.y].worldPos;
        }
        #endregion
    }
}
