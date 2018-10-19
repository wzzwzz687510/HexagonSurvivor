namespace HexagonUtils
{
    using System.Collections.Generic;
    using UnityEngine;

    public class BattleManager : MonoBehaviour
    {
        public int currentRound;
        public Entity[] entities;
        public List<int> sequence;
        public Dictionary<HexCoordinate, Entity> dirEntity = new Dictionary<HexCoordinate, Entity>();

        private void Start()
        {
            Init();
        }

        public void ChangeSequence(int index, int item)
        {
            int indexOf = sequence.IndexOf(item);
            if (indexOf == index)
                return;
            else if (indexOf > index)
            {
                sequence.Remove(item);
                sequence.Insert(index, item);
            }
            else
            {
                sequence.Insert(index, item);
                sequence.RemoveAt(indexOf);
            }
        }

        private void Init()
        {
            currentRound = 0;
        }
    }
}