namespace HexagonUtils
{
    using UnityEngine;

    public class DashMoveSkill : MoveSkill
    {
        public override void Apply(Entity caster, HexCoordinate castPosition, int skillLevel)
        {
            caster.DirectMove(castPosition);
        }
    }
}