
using UnityEngine;

public class UnifiedNetworkManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;

    public NetworkPacketManager starPacketManager;

    public MultiTopologyPacketManager multiPacketManager;

    public void SendData()
    {
        if (placementManager == null ||
            placementManager.PlacedNetwork == null)
        {
            Debug.LogWarning("Please place a topology first!");
            return;
        }

        string topologyName =
            placementManager.PlacedNetwork.name
            .Replace("(Clone)", "")
            .Trim();

        switch (topologyName)
        {
            case "StarTopology":
                if (starPacketManager != null)
                {
                    starPacketManager.SendData();
                }
                break;

            case "BusTopology":
            case "RingTopology":
                if (multiPacketManager != null)
                {
                    multiPacketManager.SendPacket();
                }
                break;

            default:
                Debug.LogWarning("Unknown topology!");
                break;
        }
    }
}
