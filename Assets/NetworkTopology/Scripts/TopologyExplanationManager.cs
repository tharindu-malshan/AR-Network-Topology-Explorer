
using UnityEngine;
using TMPro;

public class TopologyExplanationManager : MonoBehaviour
{
    public TMP_Dropdown topologyDropdown;

    public TMP_Text topologyTitle;
    public TMP_Text descriptionText;
    public TMP_Text advantagesText;
    public TMP_Text disadvantagesText;

    void Start()
    {
        UpdateExplanation();
    }

    public void UpdateExplanation()
    {
        if (topologyDropdown == null)
            return;

        switch (topologyDropdown.value)
        {
            case 0:
                ShowInfo(
                    "SELECT TOPOLOGY",
                    "Choose a network topology to learn about it.",
                    "",
                    ""
                );
                break;

            case 1:
                ShowInfo(
                    "STAR TOPOLOGY",
                    "All computers connect to a central switch.",
                    "Advantage: Easy to manage and troubleshoot.",
                    "Disadvantage: If the switch fails, the network stops."
                );
                break;

            case 2:
                ShowInfo(
                    "BUS TOPOLOGY",
                    "All computers share one main backbone cable.",
                    "Advantage: Uses less cable and is simple to install.",
                    "Disadvantage: Backbone failure can stop the network."
                );
                break;

            case 3:
                ShowInfo(
                    "RING TOPOLOGY",
                    "Computers are connected in a circular path.",
                    "Advantage: Data follows an organized transmission path.",
                    "Disadvantage: One broken connection can interrupt a simple ring."
                );
                break;

                case 4:
                ShowInfo(
            "MESH TOPOLOGY",
            "Every computer connects directly to every other computer.",
            "Advantage: Multiple paths improve reliability.",
            "Disadvantage: Requires more cables and costs more."
        );
            break;
        }
    }

    private void ShowInfo(
        string title,
        string description,
        string advantage,
        string disadvantage)
    {
        if (topologyTitle != null)
            topologyTitle.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (advantagesText != null)
            advantagesText.text = advantage;

        if (disadvantagesText != null)
            disadvantagesText.text = disadvantage;
    }
}
