using UnityEngine;

namespace BattleMap.Grid
{
    public enum GridShape
    {
        Solo = 0,
        Ver2 = 1,
        Ver3 = 2,
        Hor2 = 3,
        Hor3 = 4,
        Square = 5,
        T1 = 6,
        T2 = 7,
        CTopLeft1 = 8,
        CBottomRight1 = 9,
        CTopRight1 = 10,
        CBottomLeft1 = 11,
    }
    public static class GridService
    {
        public static Vector2Int[] ShapeToGridOffset(GridShape gridShape)
        {
            switch (gridShape)
            {
                case GridShape.Solo:
                    return new Vector2Int[] { new Vector2Int(0, 0) };
                case GridShape.Ver2:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) };
                case GridShape.Ver3:
                    return new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1) };
                case GridShape.Hor2:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) };
                case GridShape.Hor3:
                    return new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) };
                case GridShape.Square:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) };
                case GridShape.T1:
                    return new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
                case GridShape.T2:
                    return new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) };
                case GridShape.CTopLeft1:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) };
                case GridShape.CTopRight1:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
                case GridShape.CBottomLeft1:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
                case GridShape.CBottomRight1:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1) };
                default:
                    Debug.LogError("Unsupported shape");
                    return new Vector2Int[] { new Vector2Int(0, 0) };
            }
        }
    }
}
