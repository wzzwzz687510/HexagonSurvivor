namespace HexagonUtils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public enum EntityState
    {
        IDLE,
        MOVING,
        INTERACTING,
        DEAD
    }

    public enum DamageType { Normal, Block, Miss, Crit };

    [RequireComponent(typeof(Rigidbody2D))] // kinematic, only needed for OnTrigger
    [RequireComponent(typeof(NavMeshAgent2D))]
    public abstract class Entity : MonoBehaviour
    {
        [Header("Components")]
        public NavMeshAgent2D agent;
        public Animator animator;
        public ScriptableDNA m_dNA;
        new public Collider2D collider;

        [Header("State")]
        [SerializeField] EntityState _state = EntityState.IDLE;
        public EntityState state { get { return _state; } }
        public int direaction;

        [Header("Level")]
        public int level = 1;

        [Header("Health")]
        [SerializeField] protected LevelBasedInt _healthMax = new LevelBasedInt { baseValue = 100 };
        public virtual int healthMax
        {
            get
            {
                return _healthMax.Get(level);
            }
        }
        public bool invincible = false; // GMs, Npcs, ...
        int _health = 1;
        public int health
        {
            get { return Mathf.Min(_health, healthMax); } // min in case hp>hpmax after buff ends etc.
            set { _health = Mathf.Clamp(value, 0, healthMax); }
        }

        public bool healthRecovery = true; // can be disabled in combat etc.
        [SerializeField] protected LevelBasedInt _healthRecoveryRate = new LevelBasedInt { baseValue = 1 };
        public virtual int healthRecoveryRate
        {
            get
            {
                return _healthRecoveryRate.Get(level);
            }
        }

        [Header("Mana")]
        [SerializeField] protected LevelBasedInt _manaMax = new LevelBasedInt { baseValue = 100 };
        public virtual int manaMax
        {
            get
            {
                return _manaMax.Get(level);
            }
        }
        int _mana = 1;
        public int mana
        {
            get { return Mathf.Min(_mana, manaMax); } // min in case hp>hpmax after buff ends etc.
            set { _mana = Mathf.Clamp(value, 0, manaMax); }
        }

        public bool manaRecovery = true; // can be disabled in combat etc.
        public int baseManaRecoveryRate = 1;
        public int manaRecoveryRate
        {
            get
            {
                // base
                return baseManaRecoveryRate;
            }
        }

        [Header("Damage")]
        [SerializeField] protected LevelBasedInt _damage = new LevelBasedInt { baseValue = 1 };
        public virtual int damage
        {
            get
            {
                // base + buffs
                return _damage.Get(level);
            }
        }

        [Header("Defense")]
        [SerializeField] protected LevelBasedInt _defense = new LevelBasedInt { baseValue = 1 };
        public virtual int defense
        {
            get
            {
                return _defense.Get(level);
            }
        }

        [Header("Critical")]
        [SerializeField] protected LevelBasedFloat _criticalChance;
        public virtual float criticalChance
        {
            get
            {
                // base + buffs
                return _criticalChance.Get(level);
            }
        }
        [SerializeField] protected LevelBasedFloat _criticalModifier;
        public virtual float criticalModifier
        {
            get
            {
                // base + buffs
                return _criticalModifier.Get(level);
            }
        }

        // speed wrapper
        public float speed { get { return agent.speed; } }

        [Header("Damage Popup")]
        public GameObject damagePopupPrefab;

        [HideInInspector] public int currentSkill = -1;

        // effect mount is where the arrows/fireballs/etc. are spawned
        // -> can be overwritten, e.g. for mages to set it to the weapon's effect
        //    mount
        // -> assign to right hand or to self if in doubt!
        [SerializeField] Transform _effectMount;
        public virtual Transform effectMount { get { return _effectMount; } }

        // 3D text mesh for name above the entity's head
        [Header("Text Meshes")]
        public TextMesh nameOverlay;

        // look direction for animations and targetless skills
        // (NavMeshAgent itself just moves without actually looking anywhere)
        // => should always be normalized so that the animator doesn't do blending
        public Vector2 lookDirection = Vector2.down; // down by default

        /// <summary>
        /// Move related
        /// </summary>
        [HideInInspector] public HexCoordinate destination;
        [HideInInspector] public HexCoordinate currentPosition;
        protected Stack<HexCoordinate> movePath = new Stack<HexCoordinate>();
        protected HexGrid hexGrid;
        private bool isWait4NewNav;

        public List<ItemSlot> inventory;

        protected virtual void Update()
        {
            _state = UpdateState();

            if (isWait4NewNav && agent.remainingDistance < 0.01f)
            {
                movePath.Clear();
                NavigateDestination(destination);
                isWait4NewNav = false;
            }
        }

        protected bool NavigateDestination(HexCoordinate v2)
        {
            destination = v2;
            //Debug.Log("Goal("+v2.col + "," + v2.row+")");
            if (movePath.Count != 0)
            {
                isWait4NewNav = true;
                return false;
            }

            var astar = new AStarSearch(hexGrid, new Location(currentPosition), new Location(v2));
            movePath = astar.path;
            return true;
        }

        protected void SetDestination()
        {
            if (movePath.Count == 0)
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
                agent.destination = Utils.HexCoordinate2Position(hex);
                //agent.destination = SystemManager._instance.mapGenerator.dirGridEntity[hex].transform.position;
            }
        }

        public void DirectMove(HexCoordinate hex)
        {
            currentPosition = hex;
            agent.destination = Utils.HexCoordinate2Position(hex);
        }

        protected abstract EntityState UpdateState();

        public virtual void DealDamageAt(Entity entity, int amount)
        {
            int damageDealt = 0;
            DamageType damageType = DamageType.Normal;

            // don't deal any damage if entity is invincible
            if (!entity.invincible)
            {
                // block? (we use < not <= so that block rate 0 never blocks)
                if (entity.defense/amount > 2)
                {
                    damageType = DamageType.Block;
                }
                // deal damage
                else
                {
                    // critical hit?
                    if (UnityEngine.Random.value < criticalChance)
                    {
                        damageDealt = (int)Mathf.Max(criticalModifier * amount - entity.defense, 1);
                        damageType = DamageType.Crit;
                    }

                    // deal the damage
                    entity.health -= damageDealt;
                }
            }

            ShowDamagePopup(damageDealt, damageType);
        }

        void ShowDamagePopup(int amount, DamageType damageType)
        {
            // spawn the damage popup (if any) and set the text
            if (damagePopupPrefab != null)
            {
                // showing it above their head looks best, and we don't have to use
                // a custom shader to draw world space UI in front of the entity
                Bounds bounds = collider.bounds;
                Vector2 position = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

                GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
                if (damageType == DamageType.Normal)
                    popup.GetComponentInChildren<TextMesh>().text = amount.ToString();
                else if (damageType == DamageType.Block)
                    popup.GetComponentInChildren<TextMesh>().text = "<i>Block!</i>";
                else if (damageType == DamageType.Crit)
                    popup.GetComponentInChildren<TextMesh>().text = amount + " Crit!";
            }
        }

        // inventory ///////////////////////////////////////////////////////////////
        // helper function to find an item in the inventory
        public int GetInventoryIndexByName(string itemName)
        {
            return inventory.FindIndex(slot => slot.amount > 0 && slot.item.name == itemName);
        }

        // helper function to count the free slots
        public int InventorySlotsFree()
        {
            return inventory.Count(slot => slot.amount == 0);
        }

        // helper function to calculate the total amount of an item type in inventory
        // note: .Equals because name AND dynamic variables matter (petLevel etc.)
        public int InventoryCount(ScriptableItem item)
        {
            return (from slot in inventory
                    where slot.amount > 0 && slot.item.Equals(item)
                    select slot.amount).Sum();
        }

        // helper function to remove 'n' items from the inventory
        public bool InventoryRemove(ScriptableItem item, int amount)
        {
            for (int i = 0; i < inventory.Count; ++i)
            {
                ItemSlot slot = inventory[i];
                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                if (slot.amount > 0 && slot.item.Equals(item))
                {
                    // take as many as possible
                    amount -= slot.DecreaseAmount(amount);
                    inventory[i] = slot;

                    // are we done?
                    if (amount == 0) return true;
                }
            }

            // if we got here, then we didn't remove enough items
            return false;
        }

        // helper function to check if the inventory has space for 'n' items of type
        // -> the easiest solution would be to check for enough free item slots
        // -> it's better to try to add it onto existing stacks of the same type
        //    first though
        // -> it could easily take more than one slot too
        // note: this checks for one item type once. we can't use this function to
        //       check if we can add 10 potions and then 10 potions again (e.g. when
        //       doing player to player trading), because it will be the same result
        public bool InventoryCanAdd(Item item, int amount)
        {
            // go through each slot
            for (int i = 0; i < inventory.Count; ++i)
            {
                // empty? then subtract maxstack
                if (inventory[i].amount == 0)
                    amount -= item.maxStackSize;
                // not empty. same type too? then subtract free amount (max-amount)
                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                else if (inventory[i].item.Equals(item))
                    amount -= (inventory[i].item.maxStackSize - inventory[i].amount);

                // were we able to fit the whole amount already?
                if (amount <= 0) return true;
            }

            // if we got here than amount was never <= 0
            return false;
        }

        // helper function to put 'n' items of a type into the inventory, while
        // trying to put them onto existing item stacks first
        // -> this is better than always adding items to the first free slot
        // -> function will only add them if there is enough space for all of them
        public bool InventoryAdd(Item item, int amount)
        {
            // we only want to add them if there is enough space for all of them, so
            // let's double check
            if (InventoryCanAdd(item, amount))
            {
                // add to same item stacks first (if any)
                // (otherwise we add to first empty even if there is an existing
                //  stack afterwards)
                for (int i = 0; i < inventory.Count; ++i)
                {
                    // not empty and same type? then add free amount (max-amount)
                    // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                    if (inventory[i].amount > 0 && inventory[i].item.Equals(item))
                    {
                        ItemSlot temp = inventory[i];
                        amount -= temp.IncreaseAmount(amount);
                        inventory[i] = temp;
                    }

                    // were we able to fit the whole amount already? then stop loop
                    if (amount <= 0) return true;
                }

                // add to empty slots (if any)
                for (int i = 0; i < inventory.Count; ++i)
                {
                    // empty? then fill slot with as many as possible
                    if (inventory[i].amount == 0)
                    {
                        int add = Mathf.Min(amount, item.maxStackSize);
                        inventory[i] = new ItemSlot(item, add);
                        amount -= add;
                    }

                    // were we able to fit the whole amount already? then stop loop
                    if (amount <= 0) return true;
                }
                // we should have been able to add all of them
                if (amount != 0) Debug.LogError("inventory add failed: " + item.name + " " + amount);
            }
            return false;
        }

    }
}