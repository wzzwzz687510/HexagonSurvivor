namespace HexagonSurvivor
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CameraManager : MonoBehaviour
    {

        public EventSystem eventSystem;
        public LayerMask RaycastLayerMask;
        public GridEntity selectedGrid;
        public GridEntity highlightedGrid;

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
            var hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 10, RaycastLayerMask);
            if (Input.GetMouseButtonDown(1))
            {
                foreach (var hit in hits)
                {
                    if (selectedGrid)
                        selectedGrid.Resume(true);
                    selectedGrid = hit.collider.GetComponentInChildren<GridEntity>();
                    if (selectedGrid)
                        selectedGrid.Select();

                }
            }
            else
            {
                foreach (var hit in hits)
                {
                    if (highlightedGrid)
                        highlightedGrid.Resume(false);
                    highlightedGrid = hit.collider.GetComponentInChildren<GridEntity>();
                    if (highlightedGrid)
                        highlightedGrid.Highlight();
                }
            }
        }

        public bool IsOverUI()
        {
            return (eventSystem.IsPointerOverGameObject() || eventSystem.IsPointerOverGameObject(0));
        }
    }
}