using UnityEngine;

public class FramesPerSecond : MonoBehaviour
{
    private float updateInterval = 1;
    private float seconds = 0;
    private float frames  = 0;

    private string text = string.Empty;
    private GUIStyle style_label  = new GUIStyle();
    private GUIStyle style_shadow = new GUIStyle();
    private Rect pos_label  = new Rect(5, 5, 100, 25);
    private Rect pos_shadow = new Rect(5+1, 5+1, 100, 25);

    protected void Start()
    {
        Application.targetFrameRate = 60;
    }

    protected void OnGUI()
    {
        GUI.Label(pos_shadow, text, style_shadow);
        GUI.Label(pos_label, text, style_label);
    }

    protected void Update()
    {
        seconds += Time.deltaTime;
        frames++;

        if (seconds >= updateInterval)
        {
            float fps = frames/seconds;
            text = System.String.Format("{0:F2} FPS",fps);

            if (fps < 30)
                style_label.normal.textColor = Color.yellow;
            else if (fps < 10)
                style_label.normal.textColor = Color.red;
            else
                style_label.normal.textColor = Color.green;

            seconds = 0;
            frames  = 0;
        }
    }
}
