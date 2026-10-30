
using System.Collections;
using TMPro;
using UnityEngine;

public class NetworkPacketManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;
    public GameObject packetPrefab;

    public NetworkDeviceHighlighter deviceHighlighter;

    public TMP_Dropdown sourceDropdown;
    public TMP_Dropdown destinationDropdown;
    public TMP_Text statusText;
    public NetworkConnectionManager connectionManager;

    public float packetSpeed = 0.5f;

    private bool isSending = false;

    public void SendData()
    {
        if (isSending)
            return;

        if (placementManager == null ||
            !placementManager.HasPlacedNetwork)
        {
            SetStatus("Place the network first!");
            return;
        }

        if (sourceDropdown == null ||
            destinationDropdown == null ||
            packetPrefab == null)
        {
            SetStatus("Missing dropdown or packet prefab!");
            return;
        }

        string sourceName =
            sourceDropdown.options[sourceDropdown.value].text;

        string destinationName =
            destinationDropdown.options[destinationDropdown.value].text;

            Debug.Log("Selected Source: " + sourceName);
Debug.Log("Selected Destination: " + destinationName);

        if (sourceName == destinationName)
        {
            SetStatus("Select two different PCs!");
            return;
        }

        Transform network =
            placementManager.PlacedNetwork.transform;

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

        if (deviceHighlighter != null)
    {
    deviceHighlighter.Highlight(source, destination);
    }
          
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

        yield return MovePacket(packet.transform, start, middle);
        SetStatus("Switch forwarding packet to " + destinationName + "...");
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
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}
