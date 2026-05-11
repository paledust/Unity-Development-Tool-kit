using System.Collections.Generic;
using UnityEngine;

namespace BattleMap.Grid.Builder
{
    public abstract class GridBuilder<T, U> : MonoBehaviour where T : GridGraph<U> where U : IGridNode
    {
        [SerializeField] protected float nodeWidth = 1;
        [SerializeField] protected Vector2Int gridOffset;
        [SerializeField] protected Vector2Int gridSize;

        //创建一个网格图的基本步骤
        protected T BuildGrid()
        {
            //1.创建初始网格图
            var grid = CreateGraph();
            
            //2.查找场景中的节点对象并存为字典
            var nodeDict = new Dictionary<Vector2Int, U>();
            var nodes = FindObjectsByType<GridNodeObj<T, U>>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var nodeObj in nodes)
            {
                var gridPoint = grid.GetGridPointFromWorld(nodeObj.transform.position);
                nodeDict[gridPoint] = nodeObj.GetGridNode(grid);
            }
            //3.用字典刷新网格图
            grid.UpdateNode(nodeDict);
            
            //返回更新后的网格图
            return grid;
        }
        //获取到初始网格图
        protected abstract T CreateGraph();
    }
}
