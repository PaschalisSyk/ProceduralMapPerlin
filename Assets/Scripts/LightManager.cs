using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] Light directionalLight;

    [SerializeField, Range(0, 24)] float timeOfDay = 10;

    [SerializeField] Gradient ambientColor;
    [SerializeField] Gradient directionalColor;
    //[SerializeField] Material waterMat;

    private void Start()
    {
        timeOfDay = 10;
    }

    void Update()
    {
        timeOfDay += Time.deltaTime * 0.05f;
        timeOfDay %= 24;
        UpdateLighting(timeOfDay / 24);
    }

    void UpdateLighting(float timePresent)
    {
        //bool dayWater = true;
        RenderSettings.ambientLight = ambientColor.Evaluate(timePresent);

        if(directionalLight != null)
        {
            directionalLight.color = directionalColor.Evaluate(timePresent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePresent * 360f) - 90f, 170f, 0));
        }

        //if(timeOfDay > 22f || timeOfDay < 4f && dayWater )
        //{
        //    waterMat.SetFloat("_RippleSlimness" , Mathf.Lerp(waterMat.GetFloat("_RippleSlimness"), 7 , 2 * Time.deltaTime));
        //    if(waterMat.GetFloat("_RippleSlimness") == 7)
        //    {
        //        dayWater = false;
        //    }
        //}
        //else
        //{
        //    if(!dayWater)
        //    {
        //        waterMat.SetFloat("_RippleSlimness", Mathf.Lerp(waterMat.GetFloat("_RippleSlimness"), 20, 2 * Time.deltaTime));
        //        if (waterMat.GetFloat("_RippleSlimness") == 20)
        //        {
        //            dayWater = true;
        //        }
        //    }
        //}
        //waterMat.SetColor("_BaseColor", ambientColor.Evaluate(timePresent));
    }
}
