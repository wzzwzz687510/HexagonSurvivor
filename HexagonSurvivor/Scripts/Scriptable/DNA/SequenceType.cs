namespace HexagonUtils
{
    public enum Race
    {
        Dwarf,
        Elf,
        Halfling,
        Human,
        Dragonborn,
        Gnome,
        HalfElf,
        HalfOrc,
        Tiefling
    }

    public enum SequenceType
    {
        Property,
        Skill,
        Element
    }

    public enum RaceSex
    {
        Male,
        Female,
        Asexual,
        Bisexual,
        Desexual
    }

    public enum Element
    {
        Fire,
        Water,
        Earth,
        Air,

    }

    //public enum SpeciesProperty
    //{
    //    Strength,
    //    Agility,
    //    Constitution,
    //    Intelligence,
    //    Charisma,
    //    Luck
    //}

    public struct RaceProperty
    {
        public LevelBasedInt Strength, Agility, Constitution, Intelligence, Charisma, Luck, Life;
    }
}
