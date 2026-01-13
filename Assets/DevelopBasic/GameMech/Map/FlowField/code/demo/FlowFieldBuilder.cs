using System;
using BattleMap.Grid.Builder;
using UnityEngine;

namespace BattleMap.Grid.FlowField.Demo
{
    public class FlowFieldBuilder : GridBuilder<FlowField, FlowFieldNode>
    {
        [NonSerialized] private FlowField flowField = null;
        void Start()
        {
            flowField = BuildGrid();
            FlowFieldControl.Instance.Init(flowField);
        }
        protected override FlowField CreateGraph() => new FlowField(nodeWidth, gridSize, gridOffset.x, gridOffset.y);
        [ContextMenu("Refresh FlowField")]
        public void Editor_RefreshFlowField()
        {
            Start();
        }
        void OnDrawGizmosSelected()
        {
            if (FlowFieldControl.Instance != null && flowField != null)
            {
                for (int i = 0; i < gridSize.x; i++)
                {
                    for (int j = 0; j < gridSize.y; j++)
                    {
                        Vector3 position = new Vector3(gridOffset.x + i, gridOffset.y + j, 0);
                        var node = FlowFieldControl.Instance.GetNodeFromWorldPos(position);
                        Vector2 dir = node.bestDirection.Vector;
                        if (node.cost == (byte)FlowFieldNodeDirectionUtility.NodeCostType.Block)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawCube(position, Vector3.one * 0.5f);
                        }
                        else if (node.cost == (byte)FlowFieldNodeDirectionUtility.NodeCostType.Target)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawSphere(position, 0.2f);
                        }
                        else
                        {
                            Gizmos.color = Color.black;
                            Gizmos.DrawLine(position, position - (Vector3)dir.normalized * 0.3f);
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(position, position + (Vector3)dir.normalized * 0.3f);
                        }
                    }
                }
            }
            else
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < gridSize.x; i++)
                {
                    for (int j = 0; j < gridSize.y; j++)
                    {
                        Vector3 position = new Vector3(gridOffset.x + i, gridOffset.y + j, 0);
                        Gizmos.DrawSphere(position, 0.1f);
                    }
                }
            }
        }
    }
}