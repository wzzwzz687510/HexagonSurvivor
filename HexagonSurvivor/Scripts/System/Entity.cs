namespace HexagonSurvivor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))] // kinematic, only needed for OnTrigger
    [RequireComponent(typeof(NavMeshAgent2D))]
    public class Entity : MonoBehaviour
    {
        [Header("Components")]
        public NavMeshAgent2D agent;
        public Animator animator;
        new public Collider2D collider;

        // finite state machine
        // -> state only writable by entity class to avoid all kinds of confusion
        [Header("State")]
        [SerializeField] string _state = "IDLE";
        public string state { get { return _state; } }

        // 'Entity' can't be SyncVar and NetworkIdentity causes errors when null,
        // so we use [SyncVar] GameObject and wrap it for simplicity
        [Header("Target")]
        GameObject _target;
        public Entity target
        {
            get { return _target != null ? _target.GetComponent<Entity>() : null; }
            set { _target = value != null ? value.gameObject : null; }
        }

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

        [Header("Hunger")]
        [SerializeField] protected LevelBasedInt _hungerMax = new LevelBasedInt { baseValue = 100 };
        public virtual int hungerMax
        {
            get
            {
                return _hungerMax.Get(level);
            }
        }
        int _mana = 1;
        public int mana
        {
            get { return Mathf.Min(_mana, hungerMax); } // min in case hp>hpmax after buff ends etc.
            set { _mana = Mathf.Clamp(value, 0, hungerMax); }
        }

        public bool manaRecovery = true; // can be disabled in combat etc.
        public int baseHungerRecoveryRate = 1;
        public int manaRecoveryRate
        {
            get
            {
                // base
                return baseHungerRecoveryRate;
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

        public List<ItemSlot> inventory;

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
        public bool InventoryCanAdd(ScriptableItem item, int amount)
        {
            // go through each slot
            for (int i = 0; i < inventory.Count; ++i)
            {
                // empty? then subtract maxstack
                if (inventory[i].amount == 0)
                    amount -= item.maxStack;
                // not empty. same type too? then subtract free amount (max-amount)
                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                else if (inventory[i].item.Equals(item))
                    amount -= (inventory[i].item.maxStack - inventory[i].amount);

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
        public bool InventoryAdd(ScriptableItem item, int amount)
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
                        int add = Mathf.Min(amount, item.maxStack);
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