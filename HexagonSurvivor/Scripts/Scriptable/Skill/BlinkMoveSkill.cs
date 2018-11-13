namespace HexagonUtils
{
    using UnityEngine;

    public class BlinkMoveSkill : MoveSkill
    {
        public override void Apply(Entity caster, HexCoordinate castPosition, int skillLevel)
        {
            caster.transform.position = Utils.HexCoordinate2Position(castPosition);
            caster.currentPosition = castPosition;
        }
    }
}