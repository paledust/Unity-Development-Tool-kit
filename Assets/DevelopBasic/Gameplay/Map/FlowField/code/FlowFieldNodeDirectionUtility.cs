using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleMap.Grid.FlowField
{
    //节点方向工具
    public class FlowFieldNodeDirectionUtility
    {
        public enum NodeCostType
        {
            None = -1,
            Target = 0,
            Base = 1,
            Normal = 10, //普通
            Block = 255, //阻挡
        }

        public readonly Vector2Int Vector;

        private FlowFieldNodeDirectionUtility(int x, int y)
        {
            Vector = new Vector2Int(x, y);
        }

        public static implicit operator Vector2Int(FlowFieldNodeDirectionUtility direction)
        {
            return direction.Vector;
        }

        //根据Vector2Int获取方向
        public static FlowFieldNodeDirectionUtility GetDirectionFromVectorXY(Vector2Int vector)
        {
            return CardinalAndIntercardinalDirections.DefaultIfEmpty(None).FirstOrDefault(direction => direction == vector);
        }

        #region Node Directions
        public static readonly FlowFieldNodeDirectionUtility None = new FlowFieldNodeDirectionUtility(0, 0);
        public static readonly FlowFieldNodeDirectionUtility North = new FlowFieldNodeDirectionUtility(0, 1);
        public static readonly FlowFieldNodeDirectionUtility South = new FlowFieldNodeDirectionUtility(0, -1);
        public static readonly FlowFieldNodeDirectionUtility East = new FlowFieldNodeDirectionUtility(1, 0);
        public static readonly FlowFieldNodeDirectionUtility West = new FlowFieldNodeDirectionUtility(-1, 0);
        public static readonly FlowFieldNodeDirectionUtility NorthEast = new FlowFieldNodeDirectionUtility(1, 1);
        public static readonly FlowFieldNodeDirectionUtility NorthWest = new FlowFieldNodeDirectionUtility(-1, 1);
        public static readonly FlowFieldNodeDirectionUtility SouthEast = new FlowFieldNodeDirectionUtility(1, -1);
        public static readonly FlowFieldNodeDirectionUtility SouthWest = new FlowFieldNodeDirectionUtility(-1, -1);
        #endregion

        public static readonly List<FlowFieldNodeDirectionUtility> CardinalDirections = new List<FlowFieldNodeDirectionUtility>
        {
            North,
            East,
            South,
            West
        };
        public static readonly List<FlowFieldNodeDirectionUtility> CardinalAndIntercardinalDirections = new List<FlowFieldNodeDirectionUtility>
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };
        public static readonly List<FlowFieldNodeDirectionUtility> AllDirections = new List<FlowFieldNodeDirectionUtility>
        {
            None,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };
        public static bool IsAdjacent(FlowFieldNodeDirectionUtility direction)
        {
            return direction == NorthEast || direction == SouthEast || direction == SouthWest || direction == NorthWest;
        }
    }
}