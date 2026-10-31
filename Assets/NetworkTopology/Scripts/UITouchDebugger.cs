
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class UITouchDebugger : MonoBehaviour
{
    public TMP_Text debugText;

    void Update()
    {
        if (EventSystem.current == null)
            return;

        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.wasPressedThisFrame)
            return;

        Vector2 position = touch.position.ReadValue();

        PointerEventData pointerData =
            new PointerEventData(EventSystem.current);

        pointerData.position = position;

        List<RaycastResult> results =
            new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        string message = "TOUCH: " + position;

        if (results.Count > 0)
        {
            message += "\nTOP HIT: " +
                       results[0].gameObject.name;
        }
        else
        {
            message += "\nNO UI HIT";
        }

        Debug.Log(message);

        if (debugText != null)
            debugText.text = message;
    }
}
