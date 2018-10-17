namespace HexagonUtils
{
    using UnityEngine;
    using UnityEngine.Networking;
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using Mono.Data.Sqlite;

    public class Database
    {
        #region Path
#if UNITY_EDITOR
        static string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Database.sqlite");
#elif UNITY_ANDROID
    static string path = Path.Combine(Application.persistentDataPath, "Database.sqlite");
#elif UNITY_IOS
    static string path = Path.Combine(Application.persistentDataPath, "Database.sqlite");
#else
    static string path = Path.Combine(Application.dataPath, "Database.sqlite");
#endif
        #endregion

        static SqliteConnection connection;

        // constructor /////////////////////////////////////////////////////////////
        static Database()
        {
            // create database file if it doesn't exist yet
            if (!File.Exists(path))
                SqliteConnection.CreateFile(path);

            // open connection
            connection = new SqliteConnection("URI=file:" + path);
            connection.Open();

            // create tables if they don't exist yet or were deleted
            // [PRIMARY KEY is important for performance: O(log n) instead of O(n)]
            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS characters (
                            name TEXT NOT NULL PRIMARY KEY,
                            account TEXT NOT NULL,
                            class TEXT NOT NULL,
                            x INTEGER NOT NULL,
                            y INTEGER NOT NULL,
                            level INTEGER NOT NULL,
                            health INTEGER NOT NULL,
                            hunger INTEGER NOT NULL,
                            experience INTEGER NOT NULL,
                            deleted INTEGER NOT NULL)");

            // [PRIMARY KEY is important for performance: O(log n) instead of O(n)]
            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_inventory (
                            character TEXT NOT NULL,
                            slot INTEGER NOT NULL,
                            name TEXT NOT NULL,
                            amount INTEGER NOT NULL,
                            PRIMARY KEY(character, slot))");

            // [PRIMARY KEY is important for performance: O(log n) instead of O(n)]
            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS character_equipment (
                            character TEXT NOT NULL,
                            slot INTEGER NOT NULL,
                            name TEXT NOT NULL,
                            amount INTEGER NOT NULL,
                            PRIMARY KEY(character, slot))");

            // [PRIMARY KEY is important for performance: O(log n) instead of O(n)]
            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS accounts (
                            name TEXT NOT NULL PRIMARY KEY,
                            password TEXT NOT NULL,
                            banned INTEGER NOT NULL)");

            Debug.Log("connected to database");
        }

        // helper functions ////////////////////////////////////////////////////////
        // run a query that doesn't return anything
        public static void ExecuteNonQuery(string sql, params SqliteParameter[] args)
        {
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                foreach (SqliteParameter param in args)
                    command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        // run a query that returns a single value
        public static object ExecuteScalar(string sql, params SqliteParameter[] args)
        {
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                foreach (SqliteParameter param in args)
                    command.Parameters.Add(param);
                return command.ExecuteScalar();
            }
        }

        // run a query that returns several values
        // note: sqlite has long instead of int, so use Convert.ToInt32 etc.
        public static List<List<object>> ExecuteReader(string sql, params SqliteParameter[] args)
        {
            List<List<object>> result = new List<List<object>>();

            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                foreach (SqliteParameter param in args)
                    command.Parameters.Add(param);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    // the following code causes a SQL EntryPointNotFoundException
                    // because sqlite3_column_origin_name isn't found on OSX and
                    // some other platforms. newer mono versions have a workaround,
                    // but as long as Unity doesn't update, we will have to work
                    // around it manually. see also GetSchemaTable function:
                    // https://github.com/mono/mono/blob/master/mcs/class/Mono.Data.Sqlite/Mono.Data.Sqlite_2.0/SQLiteDataReader.cs
                    //
                    //result.Load(reader); (DataTable)
                    while (reader.Read())
                    {
                        object[] buffer = new object[reader.FieldCount];
                        reader.GetValues(buffer);
                        result.Add(buffer.ToList());
                    }
                }
            }

            return result;
        }

        // account data ////////////////////////////////////////////////////////////
        public static bool IsValidAccount(string account, string password)
        {

            // not empty?
            if (!Utils.IsNullOrWhiteSpace(account) && !Utils.IsNullOrWhiteSpace(password))
            {
                List<List<object>> table = ExecuteReader("SELECT password, banned FROM accounts WHERE name=@name", new SqliteParameter("@name", account));
                if (table.Count == 1)
                {
                    // account exists. check password and ban status.
                    List<object> row = table[0];
                    return (string)row[0] == password && (long)row[1] == 0;
                }
                else
                {
                    // account doesn't exist. create it.
                    ExecuteNonQuery("INSERT INTO accounts VALUES (@name, @password, 0)", new SqliteParameter("@name", account), new SqliteParameter("@password", password));
                    return true;
                }
            }
            return false;
        }

        // character data //////////////////////////////////////////////////////////
        public static bool CharacterExists(string characterName)
        {
            // checks deleted ones too so we don't end up with duplicates if we un-
            // delete one
            return ((long)ExecuteScalar("SELECT Count(*) FROM characters WHERE name=@name", new SqliteParameter("@name", characterName))) == 1;
        }

        public static void CharacterDelete(string characterName)
        {
            // soft delete the character so it can always be restored later
            ExecuteNonQuery("UPDATE characters SET deleted=1 WHERE name=@character", new SqliteParameter("@character", characterName));
        }

        // returns the list of character names for that account
        // => all the other values can be read with CharacterLoad!
        public static List<string> CharactersForAccount(string account)
        {
            List<string> result = new List<string>();
            List<List<object>> table = ExecuteReader("SELECT name FROM characters WHERE account=@account AND deleted=0", new SqliteParameter("@account", account));
            foreach (List<object> row in table)
                result.Add((string)row[0]);
            return result;
        }

        static void LoadInventory(Player player)
        {
            // fill all slots first
            for (int i = 0; i < player.inventorySize; ++i)
                player.inventory.Add(new ItemSlot());

            // then load valid items and put into their slots
            // (one big query is A LOT faster than querying each slot separately)
            List<List<object>> table = ExecuteReader("SELECT name, slot, amount, petHealth, petLevel, petExperience FROM character_inventory WHERE character=@character", new SqliteParameter("@character", player.name));
            foreach (List<object> row in table)
            {
                string itemName = (string)row[0];
                int slot = Convert.ToInt32((long)row[1]);
                ScriptableItem itemData;
                if (slot < player.inventorySize && ScriptableItem.dict.TryGetValue(itemName.GetStableHashCode(), out itemData))
                {
                    Item item = new Item(itemData);
                    int amount = Convert.ToInt32((long)row[2]);
                    player.inventory[slot] = new ItemSlot(item, amount); ;
                }
            }
        }

        //static void LoadEquipment(Player player)
        //{
        //    // fill all slots first
        //    for (int i = 0; i < player.equipmentInfo.Length; ++i)
        //        player.equipment.Add(new ItemSlot());

        //    // then load valid equipment and put into their slots
        //    // (one big query is A LOT faster than querying each slot separately)
        //    List<List<object>> table = ExecuteReader("SELECT name, slot, amount FROM character_equipment WHERE character=@character", new SqliteParameter("@character", player.name));
        //    foreach (List<object> row in table)
        //    {
        //        string itemName = (string)row[0];
        //        int slot = Convert.ToInt32((long)row[1]);
        //        ScriptableItem itemData;
        //        if (slot < player.equipmentInfo.Length && ScriptableItem.dict.TryGetValue(itemName.GetStableHashCode(), out itemData))
        //        {
        //            Item item = new Item(itemData);
        //            int amount = Convert.ToInt32((long)row[2]);
        //            player.equipment[slot] = new ItemSlot(item, amount);
        //        }
        //    }
        //}

        public static GameObject CharacterLoad(string characterName, List<Player> prefabs)
        {
            List<List<object>> table = ExecuteReader("SELECT * FROM characters WHERE name=@name AND deleted=0", new SqliteParameter("@name", characterName));
            if (table.Count == 1)
            {
                List<object> mainrow = table[0];

                // instantiate based on the class name
                string className = (string)mainrow[2];
                Player prefab = prefabs.Find(p => p.name == className);
                if (prefab != null)
                {
                    GameObject go = GameObject.Instantiate(prefab.gameObject);
                    Player player = go.GetComponent<Player>();

                    player.name = (string)mainrow[0];
                    player.account = (string)mainrow[1];
                    player.className = (string)mainrow[2];
                    float x = (float)mainrow[3];
                    float y = (float)mainrow[4];
                    Vector2 position = new Vector2(x, y);
                    player.level = Convert.ToInt32((long)mainrow[5]);
                    int health = Convert.ToInt32((long)mainrow[6]);
                    int mana = Convert.ToInt32((long)mainrow[7]);
                    player.experience = (long)mainrow[8];

                    // try to warp to loaded position.
                    // => agent.warp is recommended over transform.position and
                    //    avoids all kinds of weird bugs
                    // => warping might fail if we changed the world since last save
                    //    so we reset to start position if not on navmesh
                    player.agent.Warp(position);
                    if (!player.agent.isOnNavMesh)
                    {
                        Transform start = NetworkManager.singleton.GetNearestStartPosition(position);
                        player.agent.Warp(start.position);
                        Debug.Log(player.name + " invalid position was reset");
                    }

                    LoadInventory(player);
                    //LoadEquipment(player);

                    // assign health / mana after max values were fully loaded
                    // (they depend on equipment, buffs, etc.)
                    player.health = health;
                    player.mana = mana;
                    return go;
                }
                else Debug.LogError("no prefab found for class: " + className);
            }
            return null;
        }

        static void SaveInventory(Player player)
        {
            // inventory: remove old entries first, then add all new ones
            // (we could use UPDATE where slot=... but deleting everything makes
            //  sure that there are never any ghosts)
            ExecuteNonQuery("DELETE FROM character_inventory WHERE character=@character", new SqliteParameter("@character", player.name));
            for (int i = 0; i < player.inventory.Count; ++i)
            {
                ItemSlot slot = player.inventory[i];
                if (slot.amount > 0) // only relevant items to save queries/storage/time
                    ExecuteNonQuery("INSERT INTO character_inventory VALUES (@character, @slot, @name, @amount, @petHealth, @petLevel, @petExperience)",
                                    new SqliteParameter("@character", player.name),
                                    new SqliteParameter("@slot", i),
                                    new SqliteParameter("@name", slot.item.name),
                                    new SqliteParameter("@amount", slot.amount));
            }
        }

        //static void SaveEquipment(Player player)
        //{
        //    // equipment: remove old entries first, then add all new ones
        //    // (we could use UPDATE where slot=... but deleting everything makes
        //    //  sure that there are never any ghosts)
        //    ExecuteNonQuery("DELETE FROM character_equipment WHERE character=@character", new SqliteParameter("@character", player.name));
        //    for (int i = 0; i < player.equipment.Count; ++i)
        //    {
        //        ItemSlot slot = player.equipment[i];
        //        if (slot.amount > 0) // only relevant equip to save queries/storage/time
        //            ExecuteNonQuery("INSERT INTO character_equipment VALUES (@character, @slot, @name, @amount)",
        //                            new SqliteParameter("@character", player.name),
        //                            new SqliteParameter("@slot", i),
        //                            new SqliteParameter("@name", slot.item.name),
        //                            new SqliteParameter("@amount", slot.amount));
        //    }
        //}

        // adds or overwrites character data in the database
        public static void CharacterSave(Player player, bool online, bool useTransaction = true)
        {
            // only use a transaction if not called within SaveMany transaction
            if (useTransaction) ExecuteNonQuery("BEGIN");

            // online status:
            //   '' if offline (if just logging out etc.)
            //   current time otherwise
            // -> this way it's fault tolerant because external applications can
            //    check if online != '' and if time difference < saveinterval
            // -> online time is useful for network zones (server<->server online
            //    checks), external websites which render dynamic maps, etc.
            // -> it uses the ISO 8601 standard format
            string onlineString = online ? DateTime.UtcNow.ToString("s") : "";

            ExecuteNonQuery("INSERT OR REPLACE INTO characters VALUES (@name, @account, @class, @x, @y, @level, @health, @mana, @strength, @intelligence, @experience, @skillExperience, @gold, @coins, @online, 0)",
                            new SqliteParameter("@name", player.name),
                            new SqliteParameter("@account", player.account),
                            new SqliteParameter("@class", player.className),
                            new SqliteParameter("@x", player.transform.position.x),
                            new SqliteParameter("@y", player.transform.position.y),
                            new SqliteParameter("@level", player.level),
                            new SqliteParameter("@health", player.health),
                            new SqliteParameter("@mana", player.mana),
                            new SqliteParameter("@experience", player.experience),
                            new SqliteParameter("@online", onlineString));

            SaveInventory(player);
            //SaveEquipment(player);

            if (useTransaction) ExecuteNonQuery("END");
        }

    }
}