namespace HexagonSurvivor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Player : Entity
    {
        // some meta info
        [HideInInspector] public string account = "";
        [HideInInspector] public string className = "";

        [Header("Experience")] // note: int is not enough (can have > 2 mil. easily)
        public int maxLevel = 1;
        [SerializeField] long _experience = 0;
        public long experience
        {
            get { return _experience; }
            set
            {
                if (value <= _experience)
                {
                    // decrease
                    _experience = Math.Max(value, 0);
                }
                else
                {
                    // increase with level ups
                    // set the new value (which might be more than expMax)
                    _experience = value;

                    // now see if we leveled up (possibly more than once too)
                    // (can't level up if already max level)
                    while (_experience >= experienceMax && level < maxLevel)
                    {
                        // subtract current level's required exp, then level up
                        _experience -= experienceMax;
                        ++level;
                    }

                    // set to expMax if there is still too much exp remaining
                    if (_experience > experienceMax) _experience = experienceMax;
                }
            }
        }
        [SerializeField] protected LevelBasedLong _experienceMax = new LevelBasedLong { baseValue = 10, bonusPerLevel = 10 };
        public long experienceMax { get { return _experienceMax.Get(level); } }

        [Header("Inventory")]
        public int inventorySize = 30;
        public ScriptableItem[] defaultItems;
        public KeyCode[] inventorySplitKeys = { KeyCode.LeftShift, KeyCode.RightShift };

        [Header("Trash")]
        public ItemSlot trash = new ItemSlot();

        /// <summary>
        /// Move related
        /// </summary>
        [HideInInspector] public Vector2 destination;
        private Queue<Vector2> movePath;

        public void NavigateDestination(Vector2 v2)
        {
            agent.destination = v2;
        }

        public void SetDestination(Vector2 v2)
        {

        }

    }
}