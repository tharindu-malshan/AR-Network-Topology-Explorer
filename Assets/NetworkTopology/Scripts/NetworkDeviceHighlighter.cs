
using UnityEngine;

public class NetworkDeviceHighlighter : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public Material sourceMaterial;
    public Material destinationMaterial;

    private GameObject sourceIndicator;
    private GameObject destinationIndicator;

    public void Highlight(
        Transform source,
        Transform destination)
    {
        ClearHighlights();

        if (indicatorPrefab == null ||
            source == null ||
            destination == null)
            return;

        sourceIndicator = CreateIndicator(
            source,
            sourceMaterial
        );

        destinationIndicator = CreateIndicator(
            destination,
            destinationMaterial
        );
    }

    private GameObject CreateIndicator(
        Transform device,
        Material material)
    {
        GameObject indicator = Instantiate(
            indicatorPrefab,
            device.position + Vector3.up * 0.35f,
            Quaternion.identity,
            device
        );

        Renderer renderer = indicator.GetComponent<Renderer>();

        if (renderer != null && material != null)
            renderer.material = material;

        return indicator;
    }

    public void ClearHighlights()
    {
        if (sourceIndicator != null)
            Destroy(sourceIndicator);

        if (destinationIndicator != null)
            Destroy(destinationIndicator);
    }
}
