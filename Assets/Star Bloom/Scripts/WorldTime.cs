using UnityEngine;
using System.Collections;

public class WorldTime : Singleton<WorldTime>
{
	public enum DayTimeTypes
	{
		None = 0,
		Midnight,
		Dawn,
		Sunrise, 
     	Noon,
     	Sunset,
     	Twilight
	};

	protected WorldTimeSettings settings = null;
	protected Console console = null;

	public delegate void TimeChangedHandler( uint _val );
	public TimeChangedHandler MinuteElapsed;
	public TimeChangedHandler HourElapsed;
	public TimeChangedHandler DayElapsed;
	public TimeChangedHandler WeekElapsed;
	public TimeChangedHandler MonthElapsed;
	public TimeChangedHandler YearElapsed;

	public delegate void DayTimeChangedHandler( DayTimeTypes type );
	public DayTimeChangedHandler DayTimeChanged;


	public float Second			{ get { return settings.Second;	} }
	public float Minute			{ get { return settings.Minute;	} }
	public float Hour			{ get { return settings.Hour;	} }
	public float Day			{ get { return settings.Day;	} }
	public float Week			{ get { return settings.Week;	} }
	public float Month			{ get { return settings.Month;	} }
	public float Year			{ get { return settings.Year;	} }

	public float MidnightTime	{ get { return settings.MidnightTime;	} }
	public float DawnTime		{ get { return settings.DawnTime;		} }
	public float SunriseTime	{ get { return settings.SunriseTime;	} }
	public float NoonTime		{ get { return settings.NoonTime;		} }
	public float SunsetTime		{ get { return settings.SunsetTime;		} }
	public float TwilightTime	{ get { return settings.TwilightTime;	} }

	public DayTimeTypes CurrentTimeType = DayTimeTypes.Midnight;

	public bool TimePaused
	{ 
		get { return settings.TimePaused;	} 
		set { settings.TimePaused = value; }
	}

	// Use this for initialization
	void Start()
	{
		GameObject sceneMaster = GameObject.Find( "SceneMaster" );
		
		if( null == sceneMaster )
		{
			Debug.LogError ( "Could not find SceneMaster instance" );
			return;
		}
		
		settings = (WorldTimeSettings) sceneMaster.GetComponent( typeof(WorldTimeSettings) );
		
		if( null == settings )
		{
			Debug.LogError( "Could not find ConsoleSettings instance" );
			return;
		}

		console = Console.Instance;
		console.CommandSent += new Console.CommandEventHandler(OnCommand);

		if( settings.SecondsPerMinute == 0 )
		{
			Debug.LogError( "Invalid seconds per minute" );
			this.enabled = false;
		}

		if( settings.MinutesPerHour == 0 )
		{
			Debug.LogError( "Invalid minutes per hour" );
			this.enabled = false;
		}

		if( settings.HoursPerDay == 0 )
		{
			Debug.LogError( "Invalid hours per day" );
			this.enabled = false;
		}

		if( settings.DaysPerWeek == 0 )
		{
			Debug.LogError( "Invalid days per week" );
			this.enabled = false;
		}

		if( settings.WeeksPerMonth == 0 )
		{
			Debug.LogError( "Invalid weeks per month" );
			this.enabled = false;
		}

		if( settings.MonthsPerYear == 0 )
		{
			Debug.LogError( "Invalid months per year" );
			this.enabled = false;
		}

		if( TwilightTime < SunsetTime ||
		   SunsetTime < NoonTime ||
		   NoonTime < SunriseTime ||
		   SunriseTime < DawnTime ||
		   DawnTime < MidnightTime )
		{
			Debug.LogError( "Day times must be sequential!" );
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if( settings.TimePaused )
			return;

		// Chained functions -
		// 	Seconds
		//		- Minutes
		//			- Hours
		//				- Days
		//					- Weeks
		//						- Months
		//							- Years
		//
		// It's a little convoluted; just seemed easier to deal
		// with than a massively nested 'if' statement
		UpdateSeconds();

		UpdateDayTime();
	}

	void UpdateSeconds()
	{
		uint oldSec = (uint)settings.Second;
		settings.Second += Time.deltaTime * settings.TimeMultiplier;

		// If the second has changed
		if( (uint)settings.Second > oldSec && settings.DebugPrintType == WorldTimeSettings.DebugPrintTypes.OnSecond  )
			PrintToConsole();

		if( settings.Second >= settings.SecondsPerMinute )
		{
			uint second = (uint)settings.Second;
			uint addMins = second / (uint)settings.SecondsPerMinute;
			settings.Second -= settings.SecondsPerMinute * addMins;
			settings.Minute += addMins;
			
			OnDelegate( MinuteElapsed, addMins );
			UpdateMinutes();

			if( settings.DebugPrintType == WorldTimeSettings.DebugPrintTypes.OnMinute )
				PrintToConsole();
		}
	}

	void UpdateMinutes()
	{
		if( settings.Minute >= settings.MinutesPerHour )
		{
			uint minute = (uint)settings.Minute;
			uint addHours = minute / (uint)settings.MinutesPerHour;
			settings.Minute -= settings.MinutesPerHour * addHours;
			settings.Hour += addHours;
			
			OnDelegate( HourElapsed, addHours );
			UpdateHours();

			if( settings.DebugPrintType == WorldTimeSettings.DebugPrintTypes.OnHour )
				PrintToConsole();
		}
	}

	void UpdateHours()
	{
		if( settings.Hour >= settings.HoursPerDay )
		{
			uint hour = (uint)settings.Hour;
			uint addDays = hour / (uint)settings.HoursPerDay;
			settings.Hour -= settings.HoursPerDay * addDays;
			settings.Day += addDays;
			
			OnDelegate( DayElapsed, addDays );
			UpdateDays();
		}
	}

	void UpdateDays()
	{
		if( settings.Day >= settings.DaysPerWeek )
		{
			uint day = (uint)settings.Day;
			uint addWeeks = day / (uint)settings.DaysPerWeek;
			settings.Day -= settings.DaysPerWeek * addWeeks;
			settings.Week += addWeeks;
			
			OnDelegate( WeekElapsed, addWeeks );
			UpdateWeeks();
		}
	}

	void UpdateWeeks()
	{
		if( settings.Week >= settings.WeeksPerMonth )
		{
			uint week = (uint)settings.Week;
			uint addMonths = week / (uint)settings.WeeksPerMonth;
			settings.Week -= settings.WeeksPerMonth * addMonths;
			settings.Month += addMonths;
			
			OnDelegate( MonthElapsed, addMonths );
			UpdateMonths();
		}
	}

	void UpdateMonths()
	{
		if( settings.Month >= settings.MonthsPerYear )
		{
			uint month = (uint)settings.Month;
			uint addYears = month / (uint)settings.MonthsPerYear;
			settings.Month -= settings.MonthsPerYear * addYears;
			settings.Year += addYears;

			OnDelegate( YearElapsed, addYears );
		}
	}

	void OnDayTimeChanged( DayTimeTypes type )
	{
		if( settings.DebugPrintTransition )
			Console.Instance.addGameChatMessage( type.ToString() );

		CurrentTimeType = type;

		if( null != DayTimeChanged )
			DayTimeChanged( type );
	}

	void UpdateDayTime()
	{
		float dayPercent = GetDayPercent();

		// Tests from largest to least times
		// doing reverse order in this way is slightly faster
		if( dayPercent >= TwilightTime )
		{
			if( CurrentTimeType != DayTimeTypes.Twilight )
				OnDayTimeChanged( DayTimeTypes.Twilight );
		}
		else if( dayPercent >= SunsetTime )
		{
			if( CurrentTimeType != DayTimeTypes.Sunset )
				OnDayTimeChanged( DayTimeTypes.Sunset );
		}
		else if( dayPercent >= NoonTime )
		{
			if( CurrentTimeType != DayTimeTypes.Noon )
				OnDayTimeChanged( DayTimeTypes.Noon );
		}
		else if( dayPercent >= SunriseTime )
		{
			if( CurrentTimeType != DayTimeTypes.Sunrise )
				OnDayTimeChanged( DayTimeTypes.Sunrise );
		}
		else if( dayPercent >= DawnTime )
		{
			if( CurrentTimeType != DayTimeTypes.Dawn )
				OnDayTimeChanged( DayTimeTypes.Dawn );
		}
		else if( dayPercent >= MidnightTime )
		{
			if( CurrentTimeType != DayTimeTypes.Midnight )
				OnDayTimeChanged( DayTimeTypes.Midnight );
		}
		else if( dayPercent < MidnightTime )
		{
			// Midnight is not 0.0f
			// Edge case, transitioning from twilight to midnight
			// after the new day has started
			if( CurrentTimeType != DayTimeTypes.Twilight )
				OnDayTimeChanged( DayTimeTypes.Twilight );
		} 
	}

	void OnDelegate( TimeChangedHandler timeDelegate, uint _val )
	{
		if ( timeDelegate != null )
			timeDelegate( _val );
	}

	void OnCommand( string message )
	{
		if( message == "time" )
		{
			PrintToConsole();
		}
	}

	void PrintToConsole()
	{
		console.addGameChatMessage( ToString() );
	}

	public float GetDayAsSeconds()
	{
		float hoursToMins = settings.Hour * settings.MinutesPerHour;
		float dayToSeconds = (settings.Minute + hoursToMins) * settings.SecondsPerMinute;
		return dayToSeconds + settings.Second;
	}

	public string GetSimpleTime()
	{
		return string.Format( "{0,2:00}:{1,2:00}:{2,2:00.##}",
		                     settings.Hour,
		                     settings.Minute,
		                     settings.Second );
	}

	public string GetHourMinuteTime()
	{
		return string.Format( "{0,2:00}:{1,2:00}",
		                     settings.Hour,
		                     settings.Minute );
	}

	public float SecondsPerDay()
	{
		return settings.SecondsPerMinute * settings.MinutesPerHour * settings.HoursPerDay;
	}

	public float GetDayPercent()
	{
		return GetDayAsSeconds() / SecondsPerDay();
	}

	public override string ToString ()
	{
		return string.Format( "[WorldTime] {0}:{1}:{2:F0} ({3:F1}%) \n(Y:{4}, M:{5}, W:{6}, D:{7})",
		                     settings.Hour,
		                     settings.Minute,
		                     settings.Second,
		                     GetDayPercent()*100,
		                     settings.Year,
		                     settings.Month,
		                     settings.Week,
		                     settings.Day );
	}
}
