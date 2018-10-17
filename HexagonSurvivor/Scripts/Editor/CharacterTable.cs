namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class CharacterTable
    {

        [TableList(IsReadOnly = true), ShowInInspector]
        private readonly List<CharacterWrapper> allCharecters;

        public CharacterTable(IEnumerable<ScriptableCharacter> characters)
        {
            this.allCharecters = characters.Select(x => new CharacterWrapper(x)).ToList();
        }

        private class CharacterWrapper
        {
            private ScriptableCharacter character; // Character is a ScriptableObject and would render a unity object 
                                                   // field if drawn in the inspector, which is not what we want.

            public CharacterWrapper(ScriptableCharacter character)
            {
                this.character = character;
            }

            [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center), TableColumnWidth(50)]
            public Texture Icon { get { return this.character.Icon; } set { this.character.Icon = value; EditorUtility.SetDirty(this.character); } }

            [ShowInInspector]
            [TableColumnWidth(120)]
            public string Name { get { return this.character.Name; } set { this.character.Name = value; EditorUtility.SetDirty(this.character); } }

            [ShowInInspector]
            [ProgressBar(0, 100)]
            public float Shooting { get { return this.character.Skills.Shooting; } set { this.character.Skills.Shooting = value; EditorUtility.SetDirty(this.character); } }

            [ShowInInspector]
            [ProgressBar(0, 100)]
            public float Melee { get { return this.character.Skills.Melee; } set { this.character.Skills.Melee = value; EditorUtility.SetDirty(this.character); } }

            [ShowInInspector]
            [ProgressBar(0, 100)]
            public float Social { get { return this.character.Skills.Social; } set { this.character.Skills.Social = value; EditorUtility.SetDirty(this.character); } }

            [ShowInInspector]
            [ProgressBar(0, 100)]
            public float Animals { get { return this.character.Skills.Animals; } set { this.character.Skills.Animals = value; EditorUtility.SetDirty(this.character); } }

            [ShowInInspector]
            [ProgressBar(0, 100)]
            public float Medicine { get { return this.character.Skills.Medicine; } set { this.character.Skills.Medicine = value; EditorUtility.SetDirty(this.character); } }

            [ShowInInspector]
            [ProgressBar(0, 100)]
            public float Crafting { get { return this.character.Skills.Crafting; } set { this.character.Skills.Crafting = value; EditorUtility.SetDirty(this.character); } }
        }
    }
}
