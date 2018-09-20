namespace HexagonSurvivor
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CameraManager : MonoBehaviour
    {
        [Header("Dependent")]
        public Camera m_camera;

        [Header("LayerMask")]
        public LayerMask RaycastLayerMask;

        [HideInInspector]
        public SpriteManager selectedGrid;
        [HideInInspector]
        public SpriteManager highlightedGrid;

        [Header("Snap to Pixel Grid")]
        public float pixelsToUnits = 16;
        public int zoom = 1;
        public bool snapToGrid = true;

        [Header("Target Follow")]
        public Transform target;
        // the target position can be adjusted by an offset in order to foucs on a
        // target's head for example
        public Vector2 offset = Vector2.zero;

        // smooth the camera movement
        [Header("Dampening")]
        public float damp = 5;

        void Awake()
        {
            if (!m_camera)
            {
                Debug.Log("[CameraManager]Did't set m_camera.");
                m_camera = GetComponent<Camera>();
            }

            if (RaycastLayerMask == 0)
            {
                Debug.Log("[CameraManager]LayerMask equals zero,please check if you didn't set the LayerMask.");
            }

            if (!target)
            {
                Debug.Log("[CameraManager]Did't set target.");
                target = transform.Find("Player");
            }
        }

        void Update()
        {
            Selection();
            m_camera.orthographicSize = Screen.height / pixelsToUnits / zoom / 2;
        }

        void Selection()
        {
            if (Utils.IsCursorOverUserInterface())
            {
                return;
            }

            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, RaycastLayerMask);
            if (!hit)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {

                if (selectedGrid)
                    selectedGrid.Resume(true);
                selectedGrid = hit.collider.GetComponent<SpriteManager>();
                if (selectedGrid)
                {
                    selectedGrid.Select();
                    SystemManager._instance.OnClickMove(hit.collider.transform.position);
                }
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

        void LateUpdate()
        {
            if (!target) return;

            // calculate goal position
            Vector2 goal = (Vector2)target.position + offset;

            // interpolate
            Vector2 position = Vector2.Lerp((Vector2)transform.position, goal, Time.deltaTime * damp);

            // snap to grid, so it's always in multiples of 1/16 for pixel perfect looks
            // and to prevent shaking effects of moving objects etc.
            if (snapToGrid)
            {
                float gridSize = pixelsToUnits * zoom;
                position.x = Mathf.Round(position.x * gridSize) / gridSize;
                position.y = Mathf.Round(position.y * gridSize) / gridSize;
            }

            // convert to 3D but keep Z to stay in front of 2D plane
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    } 
}