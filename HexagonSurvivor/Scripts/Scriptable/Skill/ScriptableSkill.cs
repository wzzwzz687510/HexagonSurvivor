namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public enum CastType
    {
        None,
        Target,
        Linear,
        Ring,
        Area
    }

    public abstract class ScriptableSkill : ScriptableObject
    {
        protected const string LEFT_VERTICAL_GROUP = "Split/Left";
        protected const string Skill_BOX_GROUP = "Split/Left/Skill";
        protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";

        [HideLabel, PreviewField(55)]
        [VerticalGroup(LEFT_VERTICAL_GROUP)]
        [HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 67)]
        public Sprite Icon;

        [BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        public string Name;

        [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
        [BoxGroup("Split/Notes")]
        [HideLabel, TextArea(4, 9)]
        public string Notes;

        [BoxGroup(Skill_BOX_GROUP)]
        public ScriptableSequence[] requiredSequences;
        [BoxGroup(Skill_BOX_GROUP)]
        public CastType castType;
        [BoxGroup(Skill_BOX_GROUP)]
        public int direaction = 0;
        [BoxGroup(Skill_BOX_GROUP)]
        public int radius = 0;
        [BoxGroup(Skill_BOX_GROUP)]
        public int maxLevel = 1;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt manaCost;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt cooldown;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt castTime;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt delayTime;
        [BoxGroup(Skill_BOX_GROUP)]
        public LevelBasedInt castDistance;

        [PropertyOrder(999)]
        [BoxGroup(Skill_BOX_GROUP)]
        [HideIf("castType", CastType.Ring)]
        [HideIf("castType", CastType.Area)]
        public ScriptableSkill[] addonSkills;

        public List<HexCoordinate> targetHexs = new List<HexCoordinate>();

        public abstract void Apply(Entity caster, HexCoordinate castPosition, int skillLevel);

        public List<HexCoordinate> GetTargetHexs(HexCoordinate position)
        {
            targetHexs.Clear();
            switch (castType)
            {
                case CastType.None:
                    foreach (var item in AddonGetTargetHexs(position))
                        targetHexs.Add(item);
                    break;
                case CastType.Target:
                    targetHexs.Add(GridUtils.HexAdd(position, direaction, radius));
                    foreach (var item in AddonGetTargetHexs(GridUtils.HexAdd(position, direaction, radius)))
                        targetHexs.Add(item);
                    break;
                case CastType.Linear:
                    foreach (var item in GridUtils.HexLineDraw(position, GridUtils.HexAdd(position, direaction, radius)))
                        targetHexs.Add(item);
                    foreach (var item in AddonGetTargetHexs(GridUtils.HexAdd(position, direaction, radius)))
                        targetHexs.Add(item);
                    break;
                case CastType.Ring:
                    foreach (var item in GridUtils.HexRing(position, radius))
                        targetHexs.Add(item);
                    break;
                case CastType.Area:
                    targetHexs.Add(position);
                    foreach (var item in GridUtils.HexSpiralRings(position, radius))
                        targetHexs.Add(item);
                    break;
                default:
                    break;
            }
            Debug.Log(targetHexs.Count);
            return targetHexs;
        }

        private List<HexCoordinate> AddonGetTargetHexs(HexCoordinate position)
        {
            List<HexCoordinate> hexs = new List<HexCoordinate>();
            foreach (var addonSkill in addonSkills)
            {
                foreach (var item in addonSkill.GetTargetHexs(position))
                {
                    hexs.Add(item);
                }
            }

            return hexs;
        }

        public virtual void AddonApply(Entity caster, HexCoordinate castPosition, int skillLevel)
        {
            foreach (var addonSkill in addonSkills)
            {
                addonSkill.Apply(caster, castPosition, skillLevel);
            }
        }

        protected Entity GetTargetEntity(HexCoordinate castPosition)
        {
            Entity addonTarget = null;
            SystemManager._instance.battleManager.dirEntity.TryGetValue(GridUtils.HexAdd(castPosition, direaction, radius), out addonTarget);

            return addonTarget;
        }

        protected List<Entity> GetLinearEntities(HexCoordinate castPosition)
        {
            List<Entity> entities = new List<Entity>();
            Entity temp;
            foreach (var item in GridUtils.HexLineDraw(castPosition, GridUtils.HexAdd(castPosition, direaction, radius)))
            {
                if (SystemManager._instance.battleManager.dirEntity.TryGetValue(item, out temp))
                    entities.Add(temp);
            }

            return entities;
        }

        protected List<Entity> GetRingEntities(HexCoordinate castPosition)
        {
            List<Entity> entities = new List<Entity>();
            Entity temp;
            foreach (var item in GridUtils.HexRing(castPosition, radius))
            {
                if (SystemManager._instance.battleManager.dirEntity.TryGetValue(item, out temp))
                    entities.Add(temp);
            }

            return entities;
        }

        protected List<Entity> GetAreaEntities(HexCoordinate castPosition)
        {
            List<Entity> entities = new List<Entity>();
            Entity temp;
            if (SystemManager._instance.battleManager.dirEntity.TryGetValue(castPosition, out temp))
                entities.Add(temp);
            foreach (var item in GridUtils.HexSpiralRings(castPosition, radius))
            {
                if (SystemManager._instance.battleManager.dirEntity.TryGetValue(item, out temp))
                    entities.Add(temp);
            }

            return entities;
        }

        static Dictionary<int, ScriptableSkill> cache;
        public static Dictionary<int, ScriptableSkill> dict
        {
            get
            {
                // load if not loaded yet
                return cache ?? (cache = Resources.LoadAll<ScriptableSkill>("").ToDictionary(
                    skill => skill.name.GetStableHashCode(), skill => skill)
                );
            }
        }
    }
}
