
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class NetworkPlacementManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;

    public GameObject starTopologyPrefab;
    public GameObject busTopologyPrefab;
    public GameObject ringTopologyPrefab;
    public GameObject meshTopologyPrefab;
    public GameObject treeTopologyPrefab;

    public TMP_Dropdown topologyDropdown;

    private GameObject placedNetwork;

    private List<ARRaycastHit> hits =
        new List<ARRaycastHit>();

    public bool HasPlacedNetwork => placedNetwork != null;
    public GameObject PlacedNetwork => placedNetwork;

    void Update()
    {
        if (placedNetwork != null)
            return;

        if (!TryGetTap(out Vector2 tapPosition))
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        if (raycastManager == null)
            return;

        if (raycastManager.Raycast(
            tapPosition,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            GameObject selectedPrefab =
                GetSelectedTopologyPrefab();

            if (selectedPrefab == null)
                return;

            Pose pose = hits[0].pose;

            placedNetwork = Instantiate(
                selectedPrefab,
                pose.position,
                Quaternion.identity
            );

            Debug.Log(
                "Placed topology: " + selectedPrefab.name
            );
        }
    }

    

private GameObject GetSelectedTopologyPrefab()
{
    if (topologyDropdown == null)
    {
        Debug.LogWarning("Topology Dropdown is missing!");
        return null;
    }

    switch (topologyDropdown.value)
    {
        case 0:
            Debug.Log("Please select a topology first!");
            return null;

        case 1:
            return starTopologyPrefab;

        case 2:
            return busTopologyPrefab;

        case 3:
            return ringTopologyPrefab;

        case 4:
            return meshTopologyPrefab;

        case 5:
            return treeTopologyPrefab;

        default:
            Debug.LogWarning("Unknown topology selected!");
            return null;
    }
}


    private bool TryGetTap(out Vector2 position)
    {
        position = Vector2.zero;

        if (Touchscreen.current != null)
        {
            var touch =
                Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                position = touch.position.ReadValue();
                return true;
            }
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            position = Mouse.current.position.ReadValue();
            return true;
        }

        return false;
    }

    public void ResetPlacement()
    {
        if (placedNetwork != null)
            Destroy(placedNetwork);

        placedNetwork = null;
    }
}
