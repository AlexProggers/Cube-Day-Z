using UnityEngine;

public class SkyTestGUI : MonoBehaviour
{
    public TOD_Sky sky;
    public bool fog, sunShafts, progressTime;

    private Rect rect = new Rect(-3, -3, 200, 450+6);
    private const float label_width = 100;

    private string[] cloudTypes = {
        TOD_Weather.CloudType.None.ToString(),
        TOD_Weather.CloudType.Few.ToString(),
        TOD_Weather.CloudType.Scattered.ToString(),
        TOD_Weather.CloudType.Broken.ToString(),
        TOD_Weather.CloudType.Overcast.ToString()
    };

    private string[] weatherTypes = {
        TOD_Weather.WeatherType.Clear.ToString(),
        TOD_Weather.WeatherType.Storm.ToString(),
        TOD_Weather.WeatherType.Dust.ToString(),
        TOD_Weather.WeatherType.Fog.ToString()
    };

    protected void OnEnable()
    {
        if (!sky)
        {
            Debug.LogError("Sky instance reference not set. Disabling script.");
            this.enabled = false;
        }
    }

    protected void OnGUI()
    {
        GUILayout.BeginArea(rect, "", "Box");
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Time of Day", GUILayout.Width(label_width));
        sky.Cycle.Hour = GUILayout.HorizontalSlider(sky.Cycle.Hour, 0, 24);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Moon Phase", GUILayout.Width(label_width));
        sky.Cycle.MoonPhase = GUILayout.HorizontalSlider(sky.Cycle.MoonPhase, -1, 1);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sky Contrast", GUILayout.Width(label_width));
        sky.Atmosphere.Contrast = GUILayout.HorizontalSlider(sky.Atmosphere.Contrast, 0.5f, 1.5f);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (progressTime)
        {
            GUILayout.BeginHorizontal();
            sky.Components.Time.enabled = GUILayout.Toggle(sky.Components.Time.enabled, " Progress Time");
            GUILayout.EndHorizontal();
        }

        if (sunShafts)
        {
            GUILayout.BeginHorizontal();
            var shafts = GetComponent<TOD_SunShafts>();
            shafts.enabled = GUILayout.Toggle(shafts.enabled, " Sun Shafts");
            GUILayout.EndHorizontal();
        }

        if (fog)
        {
            GUILayout.BeginHorizontal();
            RenderSettings.fog = GUILayout.Toggle(RenderSettings.fog, " Enable Fog");
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("CLOUDS");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        int cloudType = GUILayout.SelectionGrid((int)sky.Components.Weather.Clouds - 1, cloudTypes, 1) + 1;
        sky.Components.Weather.Clouds = (TOD_Weather.CloudType)cloudType;

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("WEATHER");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        int weatherType = GUILayout.SelectionGrid((int)sky.Components.Weather.Weather - 1, weatherTypes, 1) + 1;
        sky.Components.Weather.Weather = (TOD_Weather.WeatherType)weatherType;

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
