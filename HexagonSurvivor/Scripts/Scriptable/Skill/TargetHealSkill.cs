namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class TargetHealSkill : HealSkill
    {
        [BoxGroup(Skill_BOX_GROUP)]
        public bool canHealSelf = true;
        [BoxGroup(Skill_BOX_GROUP)]
        public bool canHealOthers = false;

        public override void Apply(Entity caster, HexCoordinate castPosition, int skillLevel)
        {
            AddonApply(caster, castPosition, skillLevel);
        }
    }
}