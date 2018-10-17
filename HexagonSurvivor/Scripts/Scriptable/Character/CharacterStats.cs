namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using System;
    using UnityEngine;

    [Serializable]
    public class CharacterStats
    {
        [HideInInspector]
        public StatList Stats;

        [ProgressBar(0, 100), ShowInInspector]
        public float Shooting
        {
            get { return this.Stats[StatType.Shooting]; }
            set { this.Stats[StatType.Shooting] = value; }
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Melee
        {
            get { return this.Stats[StatType.Melee]; }
            set { this.Stats[StatType.Melee] = value; }
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Social
        {
            get { return this.Stats[StatType.Social]; }
            set { this.Stats[StatType.Social] = value; }
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Animals
        {
            get { return this.Stats[StatType.Animals]; }
            set { this.Stats[StatType.Animals] = value; }
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Medicine
        {
            get { return this.Stats[StatType.Medicine]; }
            set { this.Stats[StatType.Medicine] = value; }
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Crafting
        {
            get { return this.Stats[StatType.Crafting]; }
            set { this.Stats[StatType.Crafting] = value; }
        }
    }
}
