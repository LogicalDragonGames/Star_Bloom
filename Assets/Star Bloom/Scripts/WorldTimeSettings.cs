using UnityEngine;
using System.Collections;

public class WorldTimeSettings : MonoBehaviour
{
	public float TimeMultiplier = 100.0f;
	
	public float Year = 0;
	public float Month = 0;
	public float Week = 0;
	public float Day = 0;
	public float Hour = 0;
	public float Minute = 0;
	public float Second = 0;	
	
	public float SecondsPerMinute = 60;
	public float MinutesPerHour = 60;
	public float HoursPerDay = 24;
	public float DaysPerWeek = 7;
	public float WeeksPerMonth = 4;
	public float MonthsPerYear = 4;

	public float MidnightTime = 0.0f;
	public float DawnTime = 0.2f;
	public float SunriseTime = 0.25f;
	public float NoonTime = 0.5f;
	public float SunsetTime = 0.75f;
	public float TwilightTime = 0.9f;

	public enum DebugPrintTypes
	{
		None = 0,
		OnSecond,
		OnMinute,
		OnHour,
		OnDay
	};

	public bool TimePaused = false;
	public bool DebugPrintTransition = false;
	public DebugPrintTypes DebugPrintType;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
