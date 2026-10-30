
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiTopologyPacketManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;

    public TMP_Dropdown sourceDropdown;
    public TMP_Dropdown destinationDropdown;
    public TMP_Text packetStatusText;

    public GameObject dataPacketPrefab;

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
            SetStatus("Place a topology first!");
            return;
        }

        if (sourceDropdown == null ||
            destinationDropdown == null)
        {
            SetStatus("Dropdown references missing!");
            return;
        }

        if (sourceDropdown.value == 0 ||
    destinationDropdown.value == 0)
{
    SetStatus("Please select Source and Destination PCs!");
    return;
}

int sourceIndex = sourceDropdown.value - 1;
int destinationIndex = destinationDropdown.value - 1;

        if (sourceIndex == destinationIndex)
        {
            SetStatus("Select two different PCs!");
            return;
        }

        GameObject network = placementManager.PlacedNetwork;

        string topologyName = network.name.Replace("(Clone)", "").Trim();

        if (topologyName != "BusTopology" &&
            topologyName != "RingTopology")
        {
            SetStatus("This manager handles Bus and Ring.");
            return;
        }

        Transform sourcePC =
            network.transform.Find("PC" + (sourceIndex + 1));

        Transform destinationPC =
            network.transform.Find("PC" + (destinationIndex + 1));

        if (sourcePC == null || destinationPC == null)
        {
            SetStatus("PC objects not found!");
            return;
        }

        List<Vector3> path = topologyName == "BusTopology"
            ? BuildBusPath(network.transform, sourcePC, destinationPC)
            : BuildRingPath(network.transform, sourceIndex, destinationIndex);

        if (path.Count < 2)
        {
            SetStatus("Packet path could not be created!");
            return;
        }

        StartCoroutine(AnimatePacket(path, sourceIndex, destinationIndex));
    }

    private List<Vector3> BuildBusPath(
        Transform network,
        Transform source,
        Transform destination)
    {
        float cableY = network.TransformPoint(
            new Vector3(0f, 0.12f, 0f)).y;

        Vector3 sourcePoint = source.position + Vector3.up * packetHeight;
        Vector3 destinationPoint = destination.position + Vector3.up * packetHeight;

        Vector3 sourceBackbone = new Vector3(
            source.position.x, cableY + packetHeight,
            network.position.z);

        Vector3 destinationBackbone = new Vector3(
            destination.position.x, cableY + packetHeight,
            network.position.z);

        return new List<Vector3>
        {
            sourcePoint,
            sourceBackbone,
            destinationBackbone,
            destinationPoint
        };
    }

    private List<Vector3> BuildRingPath(
        Transform network,
        int sourceIndex,
        int destinationIndex)
    {
        List<Vector3> path = new List<Vector3>();

        int current = sourceIndex;

        while (true)
        {
            Transform pc = network.Find("PC" + (current + 1));

            if (pc == null)
                return new List<Vector3>();

            path.Add(pc.position + Vector3.up * packetHeight);

            if (current == destinationIndex)
                break;

            current = (current + 1) % 4;
        }

        return path;
    }

    private IEnumerator AnimatePacket(
        List<Vector3> path,
        int sourceIndex,
        int destinationIndex)
    {
        isSending = true;

        GameObject packet;

        if (dataPacketPrefab != null)
        {
            packet = Instantiate(
                dataPacketPrefab, path[0], Quaternion.identity);
        }
        else
        {
            packet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            packet.transform.position = path[0];
            packet.transform.localScale = Vector3.one * 0.045f;
        }

        SetStatus("Sending PC" + (sourceIndex + 1) +
                  " to PC" + (destinationIndex + 1) + "...");

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];

            float distance = Vector3.Distance(start, end);
            float duration = distance / Mathf.Max(packetSpeed, 0.01f);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                packet.transform.position =
                    Vector3.Lerp(start, end, t);

                yield return null;
            }

            packet.transform.position = end;
        }

        Destroy(packet);

        SetStatus("SUCCESS! PC" + (sourceIndex + 1) +
                  " to PC" + (destinationIndex + 1));

        isSending = false;
    }

    private void SetStatus(string message)
    {
        if (packetStatusText != null)
            packetStatusText.text = message;
              
             if (statusManager != null)
    {
    if (message.StartsWith("SUCCESS"))
        statusManager.ShowSuccess(message);
    else if (message.StartsWith("Sending"))
        statusManager.ShowWarning(message);
    else
        statusManager.ShowError(message);
    } 
        Debug.Log(message);
    }
}
