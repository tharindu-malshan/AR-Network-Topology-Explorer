
using System.Collections;
using TMPro;
using UnityEngine;

public class NetworkPacketManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;
    public GameObject packetPrefab;

    public NetworkDeviceHighlighter deviceHighlighter;
    public NetworkConnectionManager connectionManager;
    public NetworkStatusManager statusManager;

    public TMP_Dropdown sourceDropdown;
    public TMP_Dropdown destinationDropdown;
    public TMP_Text statusText;

    public float packetSpeed = 0.5f;

    private bool isSending = false;

    public void SendData()
    {
        if (isSending)
            return;

        // Check whether network is placed
        if (placementManager == null ||
            !placementManager.HasPlacedNetwork)
        {
            SetStatus("Place the network first!");
            return;
        }

        // Check required references
        if (sourceDropdown == null ||
            destinationDropdown == null ||
            packetPrefab == null)
        {
            SetStatus("Missing dropdown or packet prefab!");
            return;
        }

        // Check Select PC option
        if (sourceDropdown.value == 0 ||
            destinationDropdown.value == 0)
        {
            SetStatus("Please select Source and Destination PCs!");
            return;
        }

        // Get selected PC names
        string sourceName =
            sourceDropdown.options[sourceDropdown.value].text;

        string destinationName =
            destinationDropdown.options[destinationDropdown.value].text;

        Debug.Log("Selected Source: " + sourceName);
        Debug.Log("Selected Destination: " + destinationName);

        // Prevent sending to same PC
        if (sourceName == destinationName)
        {
            SetStatus("Select two different PCs!");
            return;
        }

        Transform network =
            placementManager.PlacedNetwork.transform;

        // This manager handles Star Topology
        if (!network.name.StartsWith("StarTopology"))
        {
            SetStatus("Please select Star Topology!");
            return;
        }

        // Find network devices
        Transform source = network.Find(sourceName);
        Transform destination = network.Find(destinationName);
        Transform networkSwitch = network.Find("NetworkSwitch");

        if (source == null ||
            destination == null ||
            networkSwitch == null)
        {
            SetStatus("Network devices not found!");
            return;
        }

        // Check source and destination connections
        if (connectionManager != null)
        {
            if (!connectionManager.IsConnected(sourceName))
            {
                SetStatus("TRANSFER FAILED! " +
                          sourceName + " is disconnected.");
                return;
            }

            if (!connectionManager.IsConnected(destinationName))
            {
                SetStatus("TRANSFER FAILED! " +
                          destinationName + " is disconnected.");
                return;
            }
        }

        // Highlight selected PCs
        if (deviceHighlighter != null)
        {
            deviceHighlighter.Highlight(source, destination);
        }

        // Start packet animation
        StartCoroutine(AnimatePacket(
            source.position + Vector3.up * 0.2f,
            networkSwitch.position + Vector3.up * 0.2f,
            destination.position + Vector3.up * 0.2f,
            sourceName,
            destinationName
        ));
    }

    private IEnumerator AnimatePacket(
        Vector3 start,
        Vector3 middle,
        Vector3 end,
        string sourceName,
        string destinationName)
    {
        isSending = true;

        SetStatus("Sending from " + sourceName + " to Switch...");

        GameObject packet = Instantiate(
            packetPrefab,
            start,
            Quaternion.identity
        );

        // Source PC to Switch
        yield return MovePacket(packet.transform, start, middle);

        SetStatus("Switch forwarding packet to " +
                  destinationName + "...");

        // Switch to Destination PC
        yield return MovePacket(packet.transform, middle, end);

        Destroy(packet);

        SetStatus("SUCCESS! Data delivered from " +
                  sourceName + " to " + destinationName);

        isSending = false;
    }

    private IEnumerator MovePacket(
        Transform packet,
        Vector3 start,
        Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        float duration = distance / Mathf.Max(packetSpeed, 0.01f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            packet.position = Vector3.Lerp(
                start,
                end,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        packet.position = end;
    }

    private void SetStatus(string message)
    {
        // Update existing status text
        if (statusText != null)
        {
            statusText.text = message;
        }

        // Update new Network Status Panel
        if (statusManager != null)
        {
            if (message.StartsWith("SUCCESS"))
            {
                statusManager.ShowSuccess(message);
            }
            else if (message.StartsWith("Sending") ||
                     message.StartsWith("Switch forwarding"))
            {
                statusManager.ShowWarning(message);
            }
            else
            {
                statusManager.ShowError(message);
            }
        }

        Debug.Log(message);
    }
}
