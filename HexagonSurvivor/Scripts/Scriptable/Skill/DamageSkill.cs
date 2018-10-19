using Sirenix.OdinInspector;
using UnityEngine;

namespace HexagonUtils
{

    public abstract class DamageSkill : ScriptableSkill
    {
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt damage;
        [BoxGroup(Skill_BOX_GROUP)]
        public OneTimeTargetSkillEffect effect;

        public void SpawnEffect(Entity caster, Entity spawnTarget)
        {
            if (effect != null)
            {
                GameObject go = Instantiate(effect.gameObject, spawnTarget.transform.position, Quaternion.identity);
                go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
                go.GetComponent<OneTimeTargetSkillEffect>().target = spawnTarget;
            }
        }
    }
}