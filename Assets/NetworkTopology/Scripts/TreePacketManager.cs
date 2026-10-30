
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreePacketManager : MonoBehaviour
{
    [Header("Managers")]
    public NetworkPlacementManager placementManager;
    public NetworkStatusManager statusManager;
    public TreeConnectionManager connectionManager;

    [Header("Packet")]
    public GameObject packetPrefab;
    public float packetSpeed = 0.5f;
    public float packetHeight = 0.06f;

    [Header("Dropdowns")]
    public TMP_Dropdown sourceDropdown;
    public TMP_Dropdown destinationDropdown;

    private bool isSending = false;

    public void SendPacket()
    {
        if (isSending)
            return;

        if (placementManager == null ||
            placementManager.PlacedNetwork == null)
        {
            ShowError("Place Tree Topology first!");
            return;
        }

        Transform network =
            placementManager.PlacedNetwork.transform;

        if (!network.name.StartsWith("TreeTopology"))
        {
            ShowError("Please select Tree Topology!");
            return;
        }

        if (connectionManager == null)
        {
            ShowError("Tree Connection Manager is missing!");
            return;
        }

        if (packetPrefab == null ||
            sourceDropdown == null ||
            destinationDropdown == null)
        {
            ShowError("Missing Tree Packet references!");
            return;
        }

        if (sourceDropdown.value == 0 ||
            destinationDropdown.value == 0)
        {
            ShowError("Select Source and Destination PCs!");
            return;
        }

        string sourceName =
            sourceDropdown.options[sourceDropdown.value].text;

        string destinationName =
            destinationDropdown.options[destinationDropdown.value].text;

        if (sourceName == destinationName)
        {
            ShowError("Select two different PCs!");
            return;
        }

        List<string> path =
            FindPath(sourceName, destinationName);

        if (path.Count == 0)
        {
            ShowError(
                "TRANSFER FAILED! No connected Tree route from " +
                sourceName + " to " + destinationName
            );
            return;
        }

        StartCoroutine(
            AnimatePacketAlongPath(network, path)
        );
    }

    private List<string> FindPath(
        string source,
        string destination)
    {
        Dictionary<string, List<string>> graph =
            new Dictionary<string, List<string>>();

        string[] nodes =
        {
            "RootSwitch",
            "SwitchA",
            "SwitchB",
            "PC1",
            "PC2",
            "PC3",
            "PC4"
        };

        foreach (string node in nodes)
            graph[node] = new List<string>();

        AddConnectedEdge(
            graph,
            "RootSwitch",
            "SwitchA",
            "TreeCable_Root_A"
        );

        AddConnectedEdge(
            graph,
            "RootSwitch",
            "SwitchB",
            "TreeCable_Root_B"
        );

        AddConnectedEdge(
            graph,
            "SwitchA",
            "PC1",
            "TreeCable_A_PC1"
        );

        AddConnectedEdge(
            graph,
            "SwitchA",
            "PC2",
            "TreeCable_A_PC2"
        );

        AddConnectedEdge(
            graph,
            "SwitchB",
            "PC3",
            "TreeCable_B_PC3"
        );

        AddConnectedEdge(
            graph,
            "SwitchB",
            "PC4",
            "TreeCable_B_PC4"
        );

        if (!graph.ContainsKey(source) ||
            !graph.ContainsKey(destination))
        {
            return new List<string>();
        }

        Queue<string> queue = new Queue<string>();
        HashSet<string> visited = new HashSet<string>();

        Dictionary<string, string> previous =
            new Dictionary<string, string>();

        queue.Enqueue(source);
        visited.Add(source);

        while (queue.Count > 0)
        {
            string current = queue.Dequeue();

            if (current == destination)
                break;

            foreach (string next in graph[current])
            {
                if (visited.Add(next))
                {
                    previous[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        if (!visited.Contains(destination))
            return new List<string>();

        List<string> path = new List<string>();

        string nodeName = destination;
        path.Add(nodeName);

        while (nodeName != source)
        {
            nodeName = previous[nodeName];
            path.Add(nodeName);
        }

        path.Reverse();

        return path;
    }

    private void AddConnectedEdge(
        Dictionary<string, List<string>> graph,
        string nodeA,
        string nodeB,
        string cableName)
    {
        if (connectionManager == null ||
            !connectionManager.IsCableConnected(cableName))
        {
            return;
        }

        graph[nodeA].Add(nodeB);
        graph[nodeB].Add(nodeA);
    }

    private IEnumerator AnimatePacketAlongPath(
        Transform network,
        List<string> path)
    {
        isSending = true;

        string route = string.Join(" -> ", path);

        if (statusManager != null)
        {
            statusManager.ShowWarning(
                "Sending: " + route
            );
        }

        Transform firstNode = network.Find(path[0]);

        if (firstNode == null)
        {
            ShowError("First Tree node not found!");
            isSending = false;
            yield break;
        }

        GameObject packet = Instantiate(
            packetPrefab,
            firstNode.position + Vector3.up * packetHeight,
            Quaternion.identity
        );

        for (int i = 0; i < path.Count - 1; i++)
        {
            Transform fromNode = network.Find(path[i]);
            Transform toNode = network.Find(path[i + 1]);

            if (fromNode == null || toNode == null)
            {
                Destroy(packet);

                ShowError("Tree node missing!");

                isSending = false;
                yield break;
            }

            Vector3 start =
                fromNode.position + Vector3.up * packetHeight;

            Vector3 end =
                toNode.position + Vector3.up * packetHeight;

            float distance = Vector3.Distance(start, end);

            float duration =
                distance / Mathf.Max(packetSpeed, 0.01f);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                packet.transform.position = Vector3.Lerp(
                    start,
                    end,
                    Mathf.Clamp01(elapsed / duration)
                );

                yield return null;
            }

            packet.transform.position = end;
        }

        Destroy(packet);

        if (statusManager != null)
        {
            statusManager.ShowSuccess(
                "SUCCESS! Tree Packet: " + route
            );
        }

        isSending = false;
    }

    private void ShowError(string message)
    {
        if (statusManager != null)
        {
            statusManager.ShowError(message);
        }

        Debug.LogWarning(message);
    }
}
