namespace HexagonUtils
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class Utils
    {
        // String.IsNullOrWhiteSpace that exists in NET4.5
        // note: can't be an extension because then it can't detect null strings
        //       like null.IsNullOrWhitespace
        public static bool IsNullOrWhiteSpace(string value)
        {
            return String.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }

        // check if the cursor is over a UI or OnGUI element right now
        // note: for UI, this only works if the UI's CanvasGroup blocks Raycasts
        // note: for OnGUI: hotControl is only set while clicking, not while zooming
        public static bool IsCursorOverUserInterface()
        {
            // IsPointerOverGameObject check for left mouse (default)
            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            // IsPointerOverGameObject check for touches
            for (int i = 0; i < Input.touchCount; ++i)
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return true;

            // OnGUI check
            return GUIUtility.hotControl != 0;
        }

        //public static HexCoordinate Position2HexCoordinate(Vector2 v2)
        //{
        //    HexCoordinate hex = new HexCoordinate();
        //    hex.row = (int)(v2.y / 1.0875f);
        //    hex.col = (int)(v2.x / 1.25f - hex.row % 2 * 0.5f);
        //    return hex;
        //}
    }
}