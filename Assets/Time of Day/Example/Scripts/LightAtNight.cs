using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightAtNight : MonoBehaviour
{
    public TOD_Sky sky;

    public  float fadeTime = 1;
    private float lerpTime = 0;

    private Light lightComponent;
    private float lightIntensity;

    protected void OnEnable()
    {
        if (!sky)
        {
            Debug.LogError("Sky instance reference not set. Disabling script.");
            this.enabled = false;
        }

        lightComponent = this.GetComponent<Light>();
        lightIntensity = lightComponent.intensity;
    }

    protected void Update()
    {
        int sign = (sky.IsNight) ? +1 : -1;
        lerpTime = Mathf.Clamp01(lerpTime + sign * Time.deltaTime / fadeTime);

        lightComponent.intensity = Mathf.Lerp(0, lightIntensity, lerpTime);
        lightComponent.enabled   = (lightComponent.intensity > 0);
    }
}
