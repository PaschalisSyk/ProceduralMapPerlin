using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] Light directionalLight;

    [SerializeField, Range(0, 24)] float timeOfDay = 10;

    [SerializeField] Gradient ambientColor;
    [SerializeField] Gradient directionalColor;


    void Update()
    {
        timeOfDay += Time.deltaTime * 0.05f;
        timeOfDay %= 24;
        UpdateLighting(timeOfDay / 24);
    }

    void UpdateLighting(float timePresent)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(timePresent);

        if(directionalLight != null)
        {
            directionalLight.color = directionalColor.Evaluate(timePresent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePresent * 360f) - 90f, 170f, 0));
        }
    }
}
