namespace HexagonSurvivor
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Player : Entity
    {
        public NavMeshAgent2D m_navMeshAgent2D;

        void Update()
        {
            //if (Input.GetMouseButton(0))
            //{
            //    Vector3 w = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    GetComponent<NavMeshAgent2D>().destination = w;
            //}
        }

    }
}