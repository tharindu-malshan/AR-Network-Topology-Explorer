
using UnityEngine;
using TMPro;

public class NetworkStatusManager : MonoBehaviour
{
    public TMP_Text statusText;

    public Color normalColor = Color.white;
    public Color successColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color errorColor = Color.red;

    void Start()
    {
        ShowNormal("Select a topology to begin");
    }

    public void ShowNormal(string message)
    {
        ShowMessage(message, normalColor);
    }

    public void ShowSuccess(string message)
    {
        ShowMessage(message, successColor);
    }

    public void ShowWarning(string message)
    {
        ShowMessage(message, warningColor);
    }

    public void ShowError(string message)
    {
        ShowMessage(message, errorColor);
    }

    private void ShowMessage(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }

        Debug.Log("Network Status: " + message);
    }
}
