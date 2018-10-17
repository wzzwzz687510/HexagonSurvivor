namespace HexagonUtils
{
    public enum Species
    {
        
    }

    public enum SequenceType
    {
        Property,
        Skill,
        Element
    }

    public enum SpeciesSex
    {
        Male,
        Female,
        Asexual,
        Bisexual
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

    public struct SpeciesProperty
    {
        public int Strength, Agility, Constitution, Intelligence, Charisma, Luck;
    }
}
