namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using System;

    [Serializable]
    public class CharacterEquipment
    {
        [ValidateInput("IsMainHand")]
        public EquipableItem MainHand;

        [ValidateInput("IsOffHand")]
        public EquipableItem Offhand;

        [ValidateInput("IsHead")]
        public EquipableItem Head;

        [ValidateInput("IsBody")]
        public EquipableItem Body;

        [ValidateInput("IsLeg")]
        public EquipableItem Leg;

        [ValidateInput("IsFoot")]
        public EquipableItem Foot;

#if UNITY_EDITOR
        private bool IsFoot(EquipableItem value)
        {
            return value == null || value.Type == ItemTypes.Foot;
        }

        private bool IsLeg(EquipableItem value)
        {
            return value == null || value.Type == ItemTypes.Leg;
        }

        private bool IsBody(EquipableItem value)
        {
            return value == null || value.Type == ItemTypes.Body;
        }

        private bool IsHead(EquipableItem value)
        {
            return value == null || value.Type == ItemTypes.Head;
        }

        private bool IsMainHand(EquipableItem value)
        {
            return value == null || value.Type == ItemTypes.MainHand;
        }

        private bool IsOffHand(EquipableItem value)
        {
            return value == null || value.Type == ItemTypes.OffHand;
        }
#endif
    }
}
