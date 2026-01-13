using System.Collections.Generic;
using UnityEngine;

namespace BattleMap.Grid.FlowField.Demo
{
    public class FlowFieldControl : Singleton<FlowFieldControl>
    {
        private FlowField flowField;

        public void Init(FlowField flowfield) => this.flowField = flowfield;
        public FlowFieldNode GetNodeFromWorldPos(Vector2 worldPos) => flowField.GetNode(worldPos);
        public Vector2Int GetGridPointFromWorld(Vector2 worldPos) => flowField.GetGridPointFromWorld(worldPos);
        public void UpdateFlowField(Dictionary<Vector2Int, byte> costDict)=>flowField.UpdateCostField(costDict);
        public void OnClear()=>flowField.ClearNodes();
    }
}
