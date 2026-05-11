using UnityEngine;

namespace BattleMap.Grid.FlowField
{
    public class FlowFieldNode : IGridNode
    {
        public Vector2Int gridXY{ get; private set; }
        public Vector2 worldPos{ get; private set; }
        public byte cost{ get; private set; }
        public ushort bestCost { get; private set; }
        public bool isTarget{ get; private set; }
        
        public FlowFieldNodeDirectionUtility bestDirection;

        public FlowFieldNode(Vector2 worldPos, Vector2Int gridXY, bool isTarget)
        {
            this.worldPos = worldPos;
            this.gridXY = gridXY;
            cost = (byte)FlowFieldNodeDirectionUtility.NodeCostType.Normal;
            bestCost = ushort.MaxValue;
            bestDirection = FlowFieldNodeDirectionUtility.None;
            this.isTarget = isTarget;
        }
        public void SetCost(byte cost)
        {
            this.cost = cost;
        }
        public void SetBestCost(ushort bestCost)
        {
            this.bestCost = bestCost;
        }
        public void SetBestDirection(FlowFieldNodeDirectionUtility bestDirection)
        {
            this.bestDirection = bestDirection;
        }
        public void ResetCost()
        {
            cost = (byte)FlowFieldNodeDirectionUtility.NodeCostType.Normal;
        }
    }
}