namespace HexagonUtils
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public enum SelectType
    {
        Normal,
        Ring,
        TriangleUp,
        TriangleDown,
        Area,
        Path
    }

    public class CameraManager : MonoBehaviour
    {
        [Header("Dependent")]
        public Camera m_camera;

        [Header("LayerMask")]
        public LayerMask RaycastLayerMask;

        [HideInInspector]
        public List<SpriteManager> selectedGrid = new List<SpriteManager>();
        [HideInInspector]
        public List<SpriteManager> highlightedGrid = new List<SpriteManager>();

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

            NormalSelect(hit);
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

        void NormalSelect(RaycastHit2D hit)
        {
            if (Input.GetMouseButtonDown(0))
            {

                SelectResume();
                SpriteManager spriteManager = hit.collider.GetComponent<SpriteManager>();
                if (spriteManager)
                {
                    selectedGrid.Add(spriteManager);
                    foreach (var item in selectedGrid)
                    {
                        if (item)
                            item.Select();
                    }
                    SystemManager._instance.OnClickMove(spriteManager.GetComponent<GridEntity>().hex);
                }
            }
            else
            {
                HighlightResume();
                GridEntity gridEntity = hit.collider.GetComponent<GridEntity>();
                if (gridEntity)
                {
                    MultiplyAdd(SelectType.Normal, gridEntity);
                    //highlightedGrid.Add(spriteManager);
                    foreach (var item in highlightedGrid)
                    {
                        if (item)
                            item.Highlight();
                    }
                }
            }
        }

        void MultiplyAdd(SelectType selectType,GridEntity gridEntity)
        {
            switch (selectType)
            {
                case SelectType.Normal:
                    highlightedGrid.Add(gridEntity.GetComponent<SpriteManager>());
                    break;
                case SelectType.Ring:
                    List<HexCoordinate> hexCoordinates = GridUtils.HexRing(gridEntity.hex, 1);
                    GridEntity tempGrid;
                    foreach (var item in hexCoordinates)
                    {
                        SystemManager._instance.mapGenerator.dirGridEntity.TryGetValue(item, out tempGrid);
                        if (tempGrid)
                            highlightedGrid.Add(tempGrid.GetComponent<SpriteManager>());
                    }       
                    break;
                case SelectType.TriangleUp:
                    break;
                case SelectType.TriangleDown:
                    break;
                case SelectType.Area:
                    break;
                case SelectType.Path:
                    break;
                default:
                    break;
            }
        }

        public void SelectResume()
        {
            if (selectedGrid.Count != 0)
            {
                foreach (var item in selectedGrid)
                {
                    if (item)
                        item.Resume(true);
                }
                selectedGrid.Clear();
            }
        }

        public void HighlightResume()
        {
            if (highlightedGrid.Count != 0)
            {
                foreach (var item in highlightedGrid)
                {
                    if (item)
                        item.Resume(false);
                }
                highlightedGrid.Clear();
            }
        }
    } 
}