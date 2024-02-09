using UnityEngine;
using UnityEngine.Events;

public class FogManager : MonoBehaviour
{
    [System.Serializable]
    public class EnvironmentEvent : UnityEvent<EnvironmentProfile> { }

    public EnvironmentEvent onEnvironmentChange;

    private MeshRenderer meshRenderer;

    private Color currentFogColor;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning("MeshRenderer component not found. Fog color will not be updated.");
        }
        else
        {
            currentFogColor = meshRenderer.material.GetColor("_FogColor");
            onEnvironmentChange.AddListener(OnEnvironmentChange);
        }
    }

    void OnEnvironmentChange(EnvironmentProfile newEnvironment)
    {
        if (meshRenderer != null)
        {
            Color newFogColor = newEnvironment.fogColor;
            if (newFogColor != currentFogColor)
            {
                meshRenderer.material.SetColor("_FogColor", newFogColor);
                currentFogColor = newFogColor;
            }
        }
    }
}

