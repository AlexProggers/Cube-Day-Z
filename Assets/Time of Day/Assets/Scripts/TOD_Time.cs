using UnityEngine;
using System;

/// Time iteration class.
///
/// Component of the sky dome parent game object.

public class TOD_Time : MonoBehaviour
{
    /// Day length inspector variable.
    /// Length of one day in minutes.
    public float DayLengthInMinutes = 30;

    /// Time update interval in seconds.
    /// Zero means every frame.
    public float UpdateInterval = 0;

    /// Date progression inspector variable.
    /// Automatically updates Cycle.Day if enabled.
    public bool ProgressDate = true;

    /// Moon phase progression inspector variable.
    /// Automatically updates Moon.Phase if enabled.
    public bool ProgressMoonPhase = true;

    private TOD_Sky sky;
    private float deltaTime;

    protected void Start()
    {
        sky = GetComponent<TOD_Sky>();
    }

    protected void Update()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime < UpdateInterval) return;

        float oneDay = DayLengthInMinutes * 60;
        float oneHour = oneDay / 24;

        float hourIter = deltaTime / oneHour;
        float moonIter = deltaTime / (30*oneDay) * 2;

        sky.Cycle.Hour += hourIter;

        if (ProgressMoonPhase)
        {
            sky.Cycle.MoonPhase += moonIter;
            if (sky.Cycle.MoonPhase < -1) sky.Cycle.MoonPhase += 2;
            else if (sky.Cycle.MoonPhase > 1) sky.Cycle.MoonPhase -= 2;
        }

        if (sky.Cycle.Hour >= 24)
        {
            sky.Cycle.Hour = 0;

            if (ProgressDate)
            {
                int daysInMonth = DateTime.DaysInMonth(sky.Cycle.Year, sky.Cycle.Month);
                if (++sky.Cycle.Day > daysInMonth)
                {
                    sky.Cycle.Day = 1;
                    if (++sky.Cycle.Month > 12)
                    {
                        sky.Cycle.Month = 1;
                        sky.Cycle.Year++;
                    }
                }
            }
        }

        deltaTime = 0;
    }

    internal void AddSeconds(float seconds, bool adjust = true)
	{
		//AddHours(seconds / 3600f);
	}

    public void SyncTime(float timeDifferenceInSec)
	{
		float num = 1440f / DayLengthInMinutes;
		AddSeconds(timeDifferenceInSec * num);
	}
}
