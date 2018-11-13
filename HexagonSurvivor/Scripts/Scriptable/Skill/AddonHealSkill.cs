namespace HexagonUtils
{
    using Sirenix.OdinInspector;

    public class AddonHealSkill : HealSkill
    {
        public override void Apply(Entity caster,HexCoordinate castPosition, int skillLevel)
        {
            switch (castType)
            {
                case CastType.None:
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
                case CastType.Target:
                    Heal(caster, GetTargetEntity(castPosition), skillLevel);
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
                case CastType.Linear:
                    foreach (var item in GetLinearEntities(castPosition))
                    {
                        Heal(caster, item, skillLevel);
                    }
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
                case CastType.Ring:
                    foreach (var item in GetRingEntities(castPosition))
                    {
                        Heal(caster, item, skillLevel);
                    }
                    break;
                case CastType.Area:
                    foreach (var item in GetAreaEntities(castPosition))
                    {
                        Heal(caster, item, skillLevel);
                    }
                    break;
                default:
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
            }
        }

        private void Heal(Entity caster, Entity target, int skillLevel)
        {
            if (!isUnionApplied && target.gameObject.layer.Equals("Enemy"))
                return;
            if (target.health > 0)
            {
                target.health += healsHealth.Get(skillLevel);
                target.mana += healsMana.Get(skillLevel);

                SpawnEffect(caster, target);
            }
        }
    }
}