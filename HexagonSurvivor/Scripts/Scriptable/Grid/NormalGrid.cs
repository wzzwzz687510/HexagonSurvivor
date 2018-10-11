namespace HexagonSurvivor
{
    using Sirenix.OdinInspector;

    public class NormalGrid : ScriptableGrid
    {

        public override GridType[] SupportedGridTypes
        {
            get
            {
                return new GridType[]
                {
                    GridType.Normal
                };
            }
        }
    }
}
