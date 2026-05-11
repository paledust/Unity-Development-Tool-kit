using System.Collections.Generic;
using UnityEngine;

namespace BattleMap.Grid.FlowField
{
    [System.Serializable]
    public class FlowField : GridGraph<FlowFieldNode>
    {
        public FlowFieldNode targetNode;
        public FlowField(float nodeWidth, Vector2Int gridXY, int offsetX, int offsetY) : base(nodeWidth, gridXY, offsetX, offsetY)
        {
            for (int x = 0; x < gridRange.x; x++)
            {
                for (int y = 0; y < gridRange.y; y++)
                {
                    Vector3 worldPos = new Vector3(nodeWidth * x + gridOffset.x, nodeWidth * y + gridOffset.y, 0);
                    nodes[x, y] = new FlowFieldNode(worldPos, new Vector2Int(x, y), false);
                }
            }
            targetNode = nodes[0, 0];
        }
        public FlowField(float nodeWidth, FlowFieldNode[,] nodes, int offsetX, int offsetY):base(nodeWidth, new Vector2Int(nodes.GetLength(0), nodes.GetLength(1)), offsetX, offsetY)
        {
            this.nodes = nodes;
            targetNode = nodes[0, 0];
        }
        protected override internal void OnNodeUpdated(Dictionary<Vector2Int, FlowFieldNode> nodeDict)
        {
            //设置目标点
            foreach (var item in nodeDict)
            {
                if (item.Value.isTarget)
                {
                    this.targetNode = nodes[item.Key.x, item.Key.y];
                    this.targetNode.SetBestCost(0);
                }
            }
            RecalculateFlowField();
        }
        //清除节点
        public void ClearNodes()
        {
            nodes = null;
            targetNode = default;
        }
        //更新成本场
        public void UpdateCostField(Dictionary<Vector2Int, byte> nodeDict)
        {
            foreach (var item in nodeDict)
            {
                nodes[item.Key.x, item.Key.y].SetCost(item.Value);
                if (item.Value == (byte)FlowFieldNodeDirectionUtility.NodeCostType.Target)
                {
                    this.targetNode = nodes[item.Key.x, item.Key.y];
                    this.targetNode.SetBestCost(0);
                }
            }
            RecalculateFlowField();
        }
        //重新计算流场
        private void RecalculateFlowField()
        {
            //更新积分场
            Queue<FlowFieldNode> listNodeToCheck = new Queue<FlowFieldNode>();

            listNodeToCheck.Enqueue(this.targetNode);

            while (listNodeToCheck.Count > 0)
            {
                FlowFieldNode frontNode = listNodeToCheck.Dequeue();

                List<FlowFieldNode> listNeighborNode = GetNeighborNodeList(frontNode.gridXY, FlowFieldNodeDirectionUtility.CardinalDirections);
                foreach (FlowFieldNode neighborNode in listNeighborNode)
                {
                    if (neighborNode.cost + frontNode.bestCost < neighborNode.bestCost)
                    {
                        neighborNode.SetBestCost((ushort)(neighborNode.cost + frontNode.bestCost));
                        listNodeToCheck.Enqueue(neighborNode);
                    }
                }
            }

            //更新流场
            foreach (FlowFieldNode nodeArgs in nodes)
            {
                List<FlowFieldNode> listNeighborNode = new List<FlowFieldNode>();

                //根据节点类型，选取相邻节点。阻挡节点会指向最近的非阻挡节点
                switch (nodeArgs.cost)
                {
                    case (byte)FlowFieldNodeDirectionUtility.NodeCostType.Block:
                        listNeighborNode = GetNeighborNodeList(nodeArgs.gridXY, FlowFieldNodeDirectionUtility.CardinalDirections);
                        break;
                    default:
                        listNeighborNode = GetNeighborNodeList(nodeArgs.gridXY, FlowFieldNodeDirectionUtility.AllDirections);
                        break;
                }
                int bestCost = nodeArgs.bestCost;

                FlowFieldNode nextNode = null;
                bool bestIsAdjacent = false;
                foreach (FlowFieldNode neighborNode in listNeighborNode)
                {
                    if (neighborNode.bestCost < bestCost)
                    {
                        nextNode = neighborNode;
                        bestCost = neighborNode.bestCost;
                        nodeArgs.SetBestDirection(FlowFieldNodeDirectionUtility.GetDirectionFromVectorXY(neighborNode.gridXY - nodeArgs.gridXY));
                        bestIsAdjacent = FlowFieldNodeDirectionUtility.IsAdjacent(nodeArgs.bestDirection);
                    }
                    //判断是否有等值的非斜线节点
                    if (neighborNode.bestCost == bestCost &&
                    bestIsAdjacent &&
                    !FlowFieldNodeDirectionUtility.IsAdjacent(FlowFieldNodeDirectionUtility.GetDirectionFromVectorXY(neighborNode.gridXY - nodeArgs.gridXY)))
                    {
                        nextNode = neighborNode;
                        bestCost = neighborNode.bestCost;
                        nodeArgs.SetBestDirection(FlowFieldNodeDirectionUtility.GetDirectionFromVectorXY(neighborNode.gridXY - nodeArgs.gridXY));
                        bestIsAdjacent = FlowFieldNodeDirectionUtility.IsAdjacent(nodeArgs.bestDirection);
                    }
                }

                if (nextNode == null || nextNode.cost == 0 || nextNode.cost == 2)
                {
                    nodeArgs.SetBestDirection(FlowFieldNodeDirectionUtility.None);
                    continue;
                }
            }
        }
        //获取邻居节点
        private List<FlowFieldNode> GetNeighborNodeList(Vector2Int nodeIndex, List<FlowFieldNodeDirectionUtility> directions)
        {
            List<FlowFieldNode> ListNeighborNode = new List<FlowFieldNode>();

            foreach (Vector2Int curDirection in directions)
            {
                //获取相相邻方向节点
                Vector2Int neighborPoint = nodeIndex + curDirection;

                if (!IsPointInGrid(neighborPoint))
                {
                    continue;
                }
                else
                    ListNeighborNode.Add(nodes[neighborPoint.x, neighborPoint.y]);

            }
            return ListNeighborNode;
        }
    }
}