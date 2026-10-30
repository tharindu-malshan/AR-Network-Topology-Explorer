
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkConnectionManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;
    public TMP_Dropdown deviceDropdown;
    public TMP_Text statusText;

    public Material connectedMaterial;
    public Material disconnectedMaterial;

    private HashSet<string> disconnectedPCs =
        new HashSet<string>();

    private Dictionary<string, Renderer> cables =
        new Dictionary<string, Renderer>();

    private GameObject cachedNetwork;

    void Update()
    {
        if (placementManager == null ||
            !placementManager.HasPlacedNetwork)
        {
            cachedNetwork = null;
            cables.Clear();
            disconnectedPCs.Clear();
            return;
        }

        GameObject network = placementManager.PlacedNetwork;

        if (network != cachedNetwork)
        {
            cachedNetwork = network;
            cables.Clear();
            disconnectedPCs.Clear();
            FindCables(network.transform);
        }
    }

    

private void FindCables(Transform network)
{
    cables.Clear();

    for (int i = 1; i <= 4; i++)
    {
        string pcName = "PC" + i;
        string cableName = "Cable_PC" + i;

        Transform cableObject = network.Find(cableName);

        if (cableObject == null)
        {
            Debug.LogWarning(cableName + " not found!");
            continue;
        }

        LineRenderer cableRenderer =
            cableObject.GetComponent<LineRenderer>();

        if (cableRenderer != null)
        {
            cables[pcName] = cableRenderer;

            Debug.Log(pcName + " cable found!");
        }
        else
        {
            Debug.LogWarning(
                cableName + " has no LineRenderer!"
            );
        }
    }
}



    public bool IsConnected(string pcName)
    {
        return !disconnectedPCs.Contains(pcName);
    }

    public void DisconnectSelectedPC()
    {
        if (!Ready())
            return;

        string pcName = GetSelectedPC();

        disconnectedPCs.Add(pcName);
        UpdateCable(pcName, false);

        SetStatus(pcName + " DISCONNECTED! Connection failed.");
    }

    public void ReconnectSelectedPC()
    {
        if (!Ready())
            return;

        string pcName = GetSelectedPC();

        disconnectedPCs.Remove(pcName);
        UpdateCable(pcName, true);

        SetStatus(pcName + " reconnected successfully.");
    }

    private void UpdateCable(string pcName, bool connected)
    {
        if (!cables.TryGetValue(pcName, out Renderer cable))
        {
            Debug.LogWarning(
                "Cable for " + pcName +
                " not found. Check cable names."
            );
            return;
        }

        Material material = connected
            ? connectedMaterial
            : disconnectedMaterial;

        if (material != null)
            cable.material = material;
    }

    private bool Ready()
    {
        if (placementManager == null ||
            !placementManager.HasPlacedNetwork)
        {
            SetStatus("Place the network first!");
            return false;
        }

        if (deviceDropdown == null)
        {
            SetStatus("Device dropdown is missing!");
            return false;
        }

        return true;
    }

    private string GetSelectedPC()
    {
        return deviceDropdown.options[
            deviceDropdown.value
        ].text;
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}
