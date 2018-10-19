namespace HexagonUtils
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct Buff
    {
        public int hash;

        public int level;
        public int buffTimeEnd; // turn time

        // constructors
        public Buff(BuffSkill data, int level)
        {
            hash = data.name.GetStableHashCode();
            this.level = level;
            // To do
            buffTimeEnd = 0;
            //buffTimeEnd = BattleManager.round + data.buffTime.Get(level); // start buff immediately
        }

        // wrappers for easier access
        public BuffSkill data { get { return (BuffSkill)ScriptableSkill.dict[hash]; } }
        public string name { get { return data.name; } }
        public Sprite image { get { return data.Icon; } }
        public int buffTime { get { return data.buffTime.Get(level); } }
        public int buffsHealthMax { get { return data.buffsHealthMax.Get(level); } }
        public int buffsManaMax { get { return data.buffsManaMax.Get(level); } }
        public int buffsDamage { get { return data.buffsDamage.Get(level); } }
        public int buffsDefense { get { return data.buffsDefense.Get(level); } }
        public float buffsCriticalChance { get { return data.buffsCriticalChance.Get(level); } }
        public float buffsHealthPercentPerSecond { get { return data.buffsHealthPercentPerRound.Get(level); } }
        public float buffsHealthPerSecond { get { return data.buffsHealthPerRound.Get(level); } }
        public float buffsManaPercentPerSecond { get { return data.buffsManaPercentPerRound.Get(level); } }
        public float buffsManaPerSecond { get { return data.buffsManaPerRound.Get(level); } }
        public int maxLevel { get { return data.maxLevel; } }
    }
}