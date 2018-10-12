namespace HexagonSurvivor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    enum CmdEvent
    {
        NavigateDestination,
        CancelAction,
        Respawn
    }

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

        HashSet<CmdEvent> cmdEvents = new HashSet<CmdEvent>();

        /// <summary>
        /// Move related
        /// </summary>
        [HideInInspector] public HexCoordinate destination;
        [HideInInspector] public HexCoordinate currentPosition;
        private Stack<HexCoordinate> movePath = new Stack<HexCoordinate>();
        private HexGrid hexGrid;
        private bool isWait4NewNav;

        private void Start()
        {
            hexGrid = new HexGrid();
        }

        protected override void Update()
        {
            base.Update();
            Debug.Log(state);
            if (isWait4NewNav && agent.remainingDistance < 0.01f)
            {
                movePath.Clear();
                NavigateDestination(destination);
                isWait4NewNav = false;
            }

            if (state == EntityState.MOVING)
                SetDestination();
        }

        protected override EntityState UpdateState()
        {
            if (state == EntityState.IDLE) return UpdateState_IDLE();
            if (state == EntityState.MOVING) return UpdateState_MOVING();
            if (state == EntityState.INTERACTING) return UpdateState_INTERACTING();
            if (state == EntityState.DEAD) return UpdateState_DEAD();
            Debug.LogError("invalid state:" + state);
            return EntityState.IDLE;
        }

        private EntityState UpdateState_IDLE()
        {
            if (EventNavigateDestination())
                return EntityState.MOVING;
            return EntityState.IDLE;
        }

        private EntityState UpdateState_MOVING()
        {
            if (EventMoveEnd())
                return EntityState.IDLE;
            return EntityState.MOVING;
        }

        private EntityState UpdateState_INTERACTING()
        {
            return EntityState.INTERACTING;
        }

        private EntityState UpdateState_DEAD()
        {
            return EntityState.DEAD;
        }

        bool EventNavigateDestination()
        { return cmdEvents.Remove(CmdEvent.NavigateDestination); }

        bool EventMoveEnd()
        { return state == EntityState.MOVING && movePath.Count == 0 && agent.remainingDistance < 0.01f; }

        public void NavigateDestination(HexCoordinate v2)
        {
            destination = v2;
            //Debug.Log("Goal("+v2.col + "," + v2.row+")");
            if (movePath.Count != 0)
            {
                isWait4NewNav = true;
                return;
            }
                
            var astar = new AStarSearch(hexGrid, new Location(currentPosition), new Location(v2));
            movePath = astar.path;
            cmdEvents.Add(CmdEvent.NavigateDestination);
        }

        public void SetDestination()
        {
            if(movePath.Count == 0)
            {
                if (agent.remainingDistance < 0.1f)
                    SystemManager._instance.cameraManager.SelectResume();
                return;
            }

            if (agent.remainingDistance < 0.01f)
            {
                HexCoordinate hex = movePath.Pop();
                //Debug.Log(hex.col + "," + hex.row);
                currentPosition = hex;
                agent.destination = SystemManager._instance.mapGenerator.dirGridEntity[hex].transform.position;
            }
        }

    }
}