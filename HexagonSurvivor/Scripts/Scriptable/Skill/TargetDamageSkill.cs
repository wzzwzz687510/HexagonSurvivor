using Sirenix.OdinInspector;

namespace HexagonUtils
{
    public class TargetDamageSkill : DamageSkill
    {
        public override void Apply(Entity caster, HexCoordinate castPosition, int skillLevel)
        {
            AddonApply(caster,castPosition, skillLevel);
        }
    }
}