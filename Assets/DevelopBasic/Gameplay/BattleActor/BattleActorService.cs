using System.Collections.Generic;
using UnityEngine;

namespace BattleActor
{
    public static class BattleActorService
    {
        public static readonly int FriendlyLayer = LayerMask.NameToLayer("Default");
        public static readonly int EnemyLayer = LayerMask.NameToLayer("Enemy");
        public static readonly Dictionary<TeamMask, LayerMask> TeamLayerMasks = new Dictionary<TeamMask, LayerMask>
        {
            { TeamMask.None, 0},
            { TeamMask.Player, 1 << FriendlyLayer },
            { TeamMask.Enemy, 1 << EnemyLayer },
            { TeamMask.Both, 1<<FriendlyLayer | 1<<EnemyLayer}
        };
        public static TeamMask GetOppositeTeam(TeamMask team) => TeamMask.Both & ~team;
        public static TeamMask GetTeam(int layer)
        {
            if (layer == FriendlyLayer)
                return TeamMask.Player;
            if (layer == EnemyLayer)
                return TeamMask.Enemy;

            return TeamMask.None;
        }
    }
}
