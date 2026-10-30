
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeshConnectionManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;
    public TMP_Dropdown connectionDropdown;
    public NetworkStatusManager statusManager;

    public Material connectedMaterial;
    public Material disconnectedMaterial;

    private readonly Dictionary<string, bool> connections =
        new Dictionary<string, bool>();

    private GameObject currentNetwork;

    private string[] cableNames =
    {
        "MeshCable_PC1_PC2",
        "MeshCable_PC1_PC3",
        "MeshCable_PC1_PC4",
        "MeshCable_PC2_PC3",
        "MeshCable_PC2_PC4",
        "MeshCable_PC3_PC4"
    };

    void Update()
    {
        GameObject network = placementManager != null
            ? placementManager.PlacedNetwork : null;

        if (network != currentNetwork)
        {
            currentNetwork = network;
            connections.Clear();

            if (network != null &&
                network.name.StartsWith("MeshTopology"))
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
        if (currentNetwork == null ||
            !currentNetwork.name.StartsWith("MeshTopology"))
        {
            ShowError("Place Mesh Topology first!");
            return;
        }

        if (connectionDropdown == null ||
            connectionDropdown.value == 0)
        {
            ShowError("Select a Mesh cable first!");
            return;
        }

        int index = connectionDropdown.value - 1;

        if (index < 0 || index >= cableNames.Length)
            return;

        string cableName = cableNames[index];

        Transform cable = currentNetwork.transform.Find(cableName);

        if (cable == null)
        {
            ShowError("Mesh cable not found: " + cableName);
            return;
        }

        LineRenderer line = cable.GetComponent<LineRenderer>();

        if (line == null)
        {
            ShowError("Line Renderer missing!");
            return;
        }

        Material material = connected
            ? connectedMaterial : disconnectedMaterial;

        if (material != null)
            line.material = material;

        connections[cableName] = connected;

        string message = connected
            ? "RECONNECTED: " + cableName
            : "DISCONNECTED: " + cableName;

        if (statusManager != null)
        {
            if (connected)
                statusManager.ShowSuccess(message);
            else
                statusManager.ShowError(message);
        }
    }

    public bool IsDirectConnectionAvailable(
        string sourceName, string destinationName)
    {
        string cable1 = "MeshCable_" +
            sourceName + "_" + destinationName;

        string cable2 = "MeshCable_" +
            destinationName + "_" + sourceName;

        if (connections.TryGetValue(cable1, out bool available))
            return available;

        if (connections.TryGetValue(cable2, out available))
            return available;

        return false;
    }

    private void ShowError(string message)
    {
        if (statusManager != null)
            statusManager.ShowError(message);

        Debug.LogWarning(message);
    }

    
public List<string> FindAlternativePath(
    string sourceName,
    string destinationName)
{
    List<string> emptyPath = new List<string>();

    if (placementManager == null ||
        placementManager.PlacedNetwork == null)
        return emptyPath;

    if (!placementManager.PlacedNetwork.name.StartsWith("MeshTopology"))
        return emptyPath;

    // Four computers in our Mesh Topology
    string[] pcs = { "PC1", "PC2", "PC3", "PC4" };

    Queue<string> queue = new Queue<string>();
    HashSet<string> visited = new HashSet<string>();
    Dictionary<string, string> previous =
        new Dictionary<string, string>();

    queue.Enqueue(sourceName);
    visited.Add(sourceName);

    while (queue.Count > 0)
    {
        string current = queue.Dequeue();

        if (current == destinationName)
            break;

        foreach (string next in pcs)
        {
            if (next == current || visited.Contains(next))
                continue;

            if (IsDirectConnectionAvailable(current, next))
            {
                visited.Add(next);
                previous[next] = current;
                queue.Enqueue(next);
            }
        }
    }

    // No connected route found
    if (!visited.Contains(destinationName))
        return emptyPath;

    List<string> path = new List<string>();

    string node = destinationName;
    path.Add(node);

    while (node != sourceName)
    {
        node = previous[node];
        path.Add(node);
    }

    path.Reverse();

    return path;
}

}
