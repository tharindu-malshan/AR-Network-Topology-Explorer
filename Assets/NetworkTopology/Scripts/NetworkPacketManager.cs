
using System.Collections;
using UnityEngine;

public class NetworkPacketManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;
    public GameObject packetPrefab;

    public float packetSpeed = 0.5f;

    private bool isSending = false;

    public void SendData()
    {
        if (isSending)
            return;

        if (placementManager == null ||
            !placementManager.HasPlacedNetwork)
        {
            Debug.Log("Place the network first!");
            return;
        }

        Transform network = placementManager.PlacedNetwork.transform;

        Transform pc1 = network.Find("PC1");
        Transform pc3 = network.Find("PC3");
        Transform networkSwitch = network.Find("NetworkSwitch");

        if (pc1 == null || pc3 == null || networkSwitch == null)
        {
            Debug.LogError("Network devices not found!");
            return;
        }

        if (packetPrefab == null)
        {
            Debug.LogError("Packet Prefab is missing!");
            return;
        }

        StartCoroutine(AnimatePacket(
            pc1.position,
            networkSwitch.position,
            pc3.position
        ));
    }

    private IEnumerator AnimatePacket(
        Vector3 start,
        Vector3 middle,
        Vector3 end)
    {
        isSending = true;

        GameObject packet = Instantiate(
            packetPrefab,
            start,
            Quaternion.identity
        );

        yield return MovePacket(packet.transform, start, middle);
        yield return MovePacket(packet.transform, middle, end);

        Destroy(packet);

        isSending = false;

        Debug.Log("Data successfully transferred from PC1 to PC3!");
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

            float t = Mathf.Clamp01(elapsed / duration);

            packet.position = Vector3.Lerp(start, end, t);

            yield return null;
        }

        packet.position = end;
    }
}
