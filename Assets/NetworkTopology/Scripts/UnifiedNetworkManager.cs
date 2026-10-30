
using UnityEngine;

public class UnifiedNetworkManager : MonoBehaviour
{
    public NetworkPlacementManager placementManager;

    public NetworkPacketManager starPacketManager;

    public MultiTopologyPacketManager multiPacketManager;

    public NetworkStatusManager statusManager;

    public MeshPacketManager meshPacketManager;

    public TreePacketManager treePacketManager;

    public void SendData()
    {
        if (placementManager == null ||
            placementManager.PlacedNetwork == null)
        {
            if (statusManager != null)
                statusManager.ShowError("Place a topology first!");

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
                    starPacketManager.SendData();
                else if (statusManager != null)
                    statusManager.ShowError("Star Packet Manager missing!");

                break;

            case "BusTopology":
            case "RingTopology":

                if (multiPacketManager != null)
                    multiPacketManager.SendPacket();
                else if (statusManager != null)
                    statusManager.ShowError("Bus/Ring Packet Manager missing!");

                break;

                case "MeshTopology":

    if (meshPacketManager != null)
    {
        meshPacketManager.SendPacket();
    }
    else if (statusManager != null)
    {
        statusManager.ShowError("Mesh Packet Manager missing!");
    }

    break;

                
        case "TreeTopology":

    if (treePacketManager != null)
    {
        treePacketManager.SendPacket();
    }
    else if (statusManager != null)
    {
        statusManager.ShowError(
            "Tree Packet Manager is missing!"
        );
    }

    break;


            default:

                if (statusManager != null)
                    statusManager.ShowError("Unknown topology!");

                break;
        }
    }
}
