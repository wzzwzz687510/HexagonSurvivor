namespace HexagonSurvivor
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CameraManager : MonoBehaviour
    {

        public EventSystem eventSystem;
        public LayerMask RaycastLayerMask;
        public SpriteManager selectedGrid;
        public SpriteManager highlightedGrid;

        public GameObject go;

        void Awake()
        {
            if (eventSystem == null)
            {
                Debug.Log("[CameraManager]Did't set the EventSystem.");
                eventSystem = GameObject.FindObjectOfType<EventSystem>();
            }

            if (RaycastLayerMask == 0)
            {
                Debug.Log("[CameraManager]LayerMask equals zero,please check if you didn't set the LayerMask.");
            }
        }

        void Update()
        {
            if (this.IsOverUI())
            {
                return;
            }

            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero, 10, RaycastLayerMask);
            if (!hit)
            {
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {

                if (selectedGrid)
                    selectedGrid.Resume(true);
                selectedGrid = hit.collider.GetComponent<SpriteManager>();
                if (selectedGrid)
                    selectedGrid.Select();
            }
            else
            {

                if (highlightedGrid)
                    highlightedGrid.Resume(false);
                highlightedGrid = hit.collider.GetComponent<SpriteManager>();
                if (highlightedGrid)
                    highlightedGrid.Highlight();

            }
        }

        public bool IsOverUI()
        {
            return (eventSystem.IsPointerOverGameObject() || eventSystem.IsPointerOverGameObject(0));
        }
    }
}