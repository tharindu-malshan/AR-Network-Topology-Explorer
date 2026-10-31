
using UnityEngine;
using TMPro;

public class TopologyControlsUI : MonoBehaviour
{
    [Header("Topology Dropdown")]
    public TMP_Dropdown topologyDropdown;

    [Header("Control Panels")]
    public GameObject starControlsPanel;
    public GameObject meshControlsPanel;
    public GameObject treeControlsPanel;

    private void Start()
    {
        if (topologyDropdown != null)
        {
            topologyDropdown.onValueChanged.AddListener(
                OnTopologyChanged
            );
        }

        UpdateControls();
    }

    private void OnTopologyChanged(int index)
    {
        UpdateControls();
    }

    public void UpdateControls()
    {
        if (topologyDropdown == null)
            return;

        int selected = topologyDropdown.value;

        if (starControlsPanel != null)
            starControlsPanel.SetActive(selected == 1);

        if (meshControlsPanel != null)
            meshControlsPanel.SetActive(selected == 4);

        if (treeControlsPanel != null)
            treeControlsPanel.SetActive(selected == 5);
    }

    private void OnDestroy()
    {
        if (topologyDropdown != null)
        {
            topologyDropdown.onValueChanged.RemoveListener(
                OnTopologyChanged
            );
        }
    }
}
