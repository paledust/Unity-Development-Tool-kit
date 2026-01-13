using BattleMap.Grid.Builder;
using UnityEngine;

namespace BattleMap.Grid.FlowField.Demo
{
    public class FlowFieldNodeObject : GridNodeObj<FlowField, FlowFieldNode>
    {
        [SerializeField] private FlowFieldNodeDirectionUtility.NodeCostType nodeCost;
        public FlowFieldNodeDirectionUtility.NodeCostType m_nodeCost => nodeCost;
        public override FlowFieldNode GetGridNode(FlowField flowField)
        {
            FlowFieldNode node = new FlowFieldNode(transform.position, flowField.GetGridPointFromWorld(transform.position), nodeCost == FlowFieldNodeDirectionUtility.NodeCostType.Target);
            node.SetCost((byte)nodeCost);
            return node;
        }
    }
}
