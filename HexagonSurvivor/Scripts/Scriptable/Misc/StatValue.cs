namespace HexagonSurvivor
{
    using Sirenix.OdinInspector;
    using System;
    using UnityEngine;

    [Serializable]
    public struct StatValue : IEquatable<StatValue>
    {
        [HideInInspector]
        public StatType Type;

        [Range(-100, 100)]
        [LabelWidth(70)]
        [LabelText("$Type")]
        public float Value;

        public StatValue(StatType type, float value)
        {
            this.Type = type;
            this.Value = value;
        }

        public StatValue(StatType type)
        {
            this.Type = type;
            this.Value = 0;
        }

        public bool Equals(StatValue other)
        {
            return this.Type == other.Type && this.Value == other.Value;
        }
    }
}