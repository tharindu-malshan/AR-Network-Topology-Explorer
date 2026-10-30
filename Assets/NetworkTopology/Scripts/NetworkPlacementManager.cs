
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class NetworkPlacementManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject starTopologyPrefab;

    private GameObject placedNetwork;
    public GameObject PlacedNetwork => placedNetwork;

    private readonly List<ARRaycastHit> hits =
        new List<ARRaycastHit>();

    public bool HasPlacedNetwork => placedNetwork != null;

    void Update()
    {
        if (placedNetwork != null)
            return;

        if (!TryGetTapPosition(out Vector2 screenPosition))
            return;

        if (raycastManager == null || starTopologyPrefab == null)
            return;

        if (raycastManager.Raycast(
            screenPosition,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            placedNetwork = Instantiate(
                starTopologyPrefab,
                hitPose.position,
                Quaternion.identity
            );

            Debug.Log("Star Topology placed successfully!");
        }
        else
        {
            Debug.Log("No AR plane detected at tap position.");
        }
    }

    private bool TryGetTapPosition(out Vector2 position)
    {
        position = Vector2.zero;

        // Android touchscreen
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                position = touch.position.ReadValue();
                return true;
            }
        }

        // Mouse for XR Simulation
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            position = Mouse.current.position.ReadValue();
            return true;
        }

        return false;
    }
}
