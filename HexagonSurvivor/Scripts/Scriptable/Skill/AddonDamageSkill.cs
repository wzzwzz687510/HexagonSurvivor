using Sirenix.OdinInspector;
namespace HexagonUtils
{
    public class AddonDamageSkill : DamageSkill
    {
        public override void Apply(Entity caster, HexCoordinate castPosition, int skillLevel)
        {
            switch (castType)
            {
                case CastType.None:
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
                case CastType.Target:
                    Damage(caster, GetTargetEntity(castPosition), skillLevel);
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
                case CastType.Linear:
                    foreach (var item in GetLinearEntities(castPosition))
                    {
                        Damage(caster, item, skillLevel);
                    }
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
                case CastType.Ring:
                    foreach (var item in GetRingEntities(castPosition))
                    {
                        Damage(caster, item, skillLevel);
                    }
                    break;
                case CastType.Area:
                    foreach (var item in GetAreaEntities(castPosition))
                    {
                        Damage(caster, item, skillLevel);
                    }
                    break;
                default:
                    AddonApply(caster, GridUtils.HexAdd(castPosition, direaction, radius), skillLevel);
                    break;
            }
        }

        private void Damage(Entity caster, Entity target, int skillLevel)
        {
            if (isUnionApplied && target.gameObject.layer.Equals("Union"))
            {
                caster.DealDamageAt(target, caster.damage + damage.Get(skillLevel));
                SpawnEffect(caster, target);
            }
        }
    }
}
