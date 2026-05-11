using UnityEngine;

namespace BattleMap.Grid.Builder
{
    public abstract class GridNodeObj<T, U> : MonoBehaviour where T:GridGraph<U> where U : IGridNode
    {
        public abstract U GetGridNode(T gridGraph);
    }
}
