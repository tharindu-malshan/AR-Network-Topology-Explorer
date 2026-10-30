using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class MeshPacketManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;
    public GameObject packetPrefab;

    public TMP_Dropdown sourceDropdown;
    public TMP_Dropdown destinationDropdown;

    public MeshConnectionManager connectionManager;

    public NetworkStatusManager statusManager;

    public float packetSpeed = 0.5f;
    public float packetHeight = 0.06f;

    private bool isSending = false;

    public void SendPacket()
    {
        if (isSending)
            return;

        if (placementManager == null ||
            placementManager.PlacedNetwork == null)
        {
            ShowError("Place Mesh Topology first!");
            return;
        }

        Transform network = placementManager.PlacedNetwork.transform;

        if (!network.name.StartsWith("MeshTopology"))
        {
            ShowError("Please select Mesh Topology!");
            return;
        }

        if (sourceDropdown == null ||
            destinationDropdown == null ||
            packetPrefab == null)
        {
            ShowError("Missing dropdown or packet prefab!");
            return;
        }

        // Index 0 = Select PC
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

        Transform source = network.Find(sourceName);
        Transform destination = network.Find(destinationName);

        if (source == null || destination == null)
        {
            ShowError("Mesh PCs not found!");
            return;
        }
        
if (connectionManager == null)
{
    ShowError("Mesh Connection Manager is missing!");
    return;
}

// Find a working route through connected cables
List<string> path = connectionManager.FindAlternativePath(
    sourceName,
    destinationName
);

if (path.Count == 0)
{
    ShowError(
        "TRANSFER FAILED! No available route from " +
        sourceName + " to " + destinationName
    );
    return;
}

// Start packet animation using the selected route
StartCoroutine(AnimatePacketAlongPath(
    network,
    path
));

    }

    private IEnumerator AnimatePacket(
        Vector3 start,
        Vector3 end,
        string sourceName,
        string destinationName)
    {
        isSending = true;

        if (statusManager != null)
            statusManager.ShowWarning(
                "Sending directly from " + sourceName +
                " to " + destinationName + "...");

        GameObject packet = Instantiate(
            packetPrefab,
            start,
            Quaternion.identity
        );

        float distance = Vector3.Distance(start, end);
        float duration = distance / Mathf.Max(packetSpeed, 0.01f);
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

        Destroy(packet);

        if (statusManager != null)
            statusManager.ShowSuccess(
                "SUCCESS! " + sourceName +
                " to " + destinationName +
                " (Direct Mesh Connection)");

        isSending = false;
    }

    private void ShowError(string message)
    {
        if (statusManager != null)
            statusManager.ShowError(message);

        Debug.LogWarning(message);
    }

    
private IEnumerator AnimatePacketAlongPath(
    Transform network,
    List<string> path)
{
    isSending = true;

    string route = string.Join(" → ", path);

    if (statusManager != null)
    {
        statusManager.ShowWarning(
            path.Count > 2
                ? "REROUTING: " + route
                : "Sending directly: " + route
        );
    }

    Transform firstPC = network.Find(path[0]);

    if (firstPC == null)
    {
        ShowError("Source PC not found!");
        isSending = false;
        yield break;
    }

    GameObject packet = Instantiate(
        packetPrefab,
        firstPC.position + Vector3.up * packetHeight,
        Quaternion.identity
    );

    for (int i = 0; i < path.Count - 1; i++)
    {
        Transform fromPC = network.Find(path[i]);
        Transform toPC = network.Find(path[i + 1]);

        if (fromPC == null || toPC == null)
        {
            Destroy(packet);
            ShowError("A PC in the route was not found!");
            isSending = false;
            yield break;
        }

        Vector3 start =
            fromPC.position + Vector3.up * packetHeight;

        Vector3 end =
            toPC.position + Vector3.up * packetHeight;

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
            path.Count > 2
                ? "SUCCESS! Rerouted via " + route
                : "SUCCESS! Direct Mesh Connection: " + route
        );
    }

    isSending = false;
}

}
