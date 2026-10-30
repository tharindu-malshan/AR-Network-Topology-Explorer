
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreeConnectionManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;
    public TMP_Dropdown connectionDropdown;
    public NetworkStatusManager statusManager;

    public Material connectedMaterial;
    public Material disconnectedMaterial;

    private readonly string[] cableNames =
    {
        "TreeCable_Root_A",
        "TreeCable_Root_B",
        "TreeCable_A_PC1",
        "TreeCable_A_PC2",
        "TreeCable_B_PC3",
        "TreeCable_B_PC4"
    };

    private Dictionary<string, bool> connections =
        new Dictionary<string, bool>();

    private GameObject currentNetwork;

    void Update()
    {
        GameObject network = placementManager != null
            ? placementManager.PlacedNetwork
            : null;

        if (network != currentNetwork)
        {
            currentNetwork = network;
            connections.Clear();

            if (network != null &&
                network.name.StartsWith("TreeTopology"))
            {
                foreach (string cable in cableNames)
                    connections[cable] = true;
            }
        }
    }

    public void DisconnectCable()
    {
        ChangeConnection(false);
    }

    public void ReconnectCable()
    {
        ChangeConnection(true);
    }

    private void ChangeConnection(bool connected)
    {
        GameObject network = placementManager != null
            ? placementManager.PlacedNetwork
            : null;

        if (network == null ||
            !network.name.StartsWith("TreeTopology"))
        {
            ShowError("Place Tree Topology first!");
            return;
        }

        if (connectionDropdown == null ||
            connectionDropdown.value == 0)
        {
            ShowError("Select a Tree cable first!");
            return;
        }

        int index = connectionDropdown.value - 1;

        if (index < 0 || index >= cableNames.Length)
        {
            ShowError("Invalid Tree cable selected!");
            return;
        }

        string cableName = cableNames[index];

        Transform cable = network.transform.Find(cableName);

        if (cable == null)
        {
            ShowError("Cable not found: " + cableName);
            return;
        }

        LineRenderer line = cable.GetComponent<LineRenderer>();

        if (line == null)
        {
            ShowError("LineRenderer missing: " + cableName);
            return;
        }

        if (connected)
        {
            if (connectedMaterial != null)
                line.material = connectedMaterial;
        }
        else
        {
            if (disconnectedMaterial != null)
                line.material = disconnectedMaterial;
        }

        connections[cableName] = connected;

        if (statusManager != null)
        {
            if (connected)
                statusManager.ShowSuccess(
                    "RECONNECTED: " + cableName);
            else
                statusManager.ShowError(
                    "DISCONNECTED: " + cableName);
        }
    }

    public bool IsCableConnected(string cableName)
    {
        GameObject network = placementManager != null
            ? placementManager.PlacedNetwork
            : null;

        if (network == null ||
            !network.name.StartsWith("TreeTopology"))
            return false;

        return connections.TryGetValue(
            cableName, out bool connected) && connected;
    }

    private void ShowError(string message)
    {
        if (statusManager != null)
            statusManager.ShowError(message);

        Debug.LogWarning(message);
    }
}
