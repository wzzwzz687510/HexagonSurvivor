using Sirenix.OdinInspector;

namespace HexagonUtils
{

    public abstract class BuffSkill : ScriptableSkill
    {
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt buffTime;

        // Properties
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt buffsHealthMax;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt buffsManaMax;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt buffsDamage;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt buffsDefense;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedFloat buffsCriticalChance; // range [0,1]
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedFloat buffsHealthPercentPerRound; // 0.1=10%; can be negative too
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt buffsHealthPerRound; 
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedFloat buffsManaPercentPerRound; // 0.1=10%; can be negative too
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedFloat buffsManaPerRound; 
    }
}