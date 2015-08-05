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
	public event TimeChangedHandler MinuteElapsed;
	public event TimeChangedHandler HourElapsed;
	public event TimeChangedHandler DayElapsed;
	public event TimeChangedHandler WeekElapsed;
	public event TimeChangedHandler MonthElapsed;
	public event TimeChangedHandler YearElapsed;
	public event TimeChangedHandler GameDayElapsed;

	public delegate void DayTimeChangedHandler( DayTimeTypes type );
	public DayTimeChangedHandler DayTimeChanged;


	public float Second				{ get { return settings.Second;	} }
	public float Minute				{ get { return settings.Minute;	} }
	public float Hour				{ get { return settings.Hour;	} }
	public float Day				{ get { return settings.Day;	} }
	public float Week				{ get { return settings.Week;	} }
	public float Month				{ get { return settings.Month;	} }
	public float Year				{ get { return settings.Year;	} }

	protected float m_PrevGameDay = -1;
	public float GameDay			{ get { return settings.GameDay;			} }
	public float GameDayLapseTime	{ get { return settings.GameDayLapseTime;	} }

	public float MidnightTime		{ get { return settings.MidnightTime;	} }
	public float DawnTime			{ get { return settings.DawnTime;		} }
	public float SunriseTime		{ get { return settings.SunriseTime;	} }
	public float NoonTime			{ get { return settings.NoonTime;		} }
	public float SunsetTime			{ get { return settings.SunsetTime;		} }
	public float TwilightTime		{ get { return settings.TwilightTime;	} }

	[HideInInspector]
	public float SecondsPerMinute	{ get { return settings.SecondsPerMinute;	} }

	[HideInInspector]
	public float MinutesPerHour		{ get { return settings.MinutesPerHour;		} }

	[HideInInspector]
	public float HoursPerDay		{ get { return settings.HoursPerDay;		} }
	
	[HideInInspector]
	public float DaysPerWeek		{ get { return settings.DaysPerWeek;		} }
	
	[HideInInspector]
	public float WeeksPerMonth		{ get { return settings.WeeksPerMonth;		} }
	
	[HideInInspector]
	public float MonthsPerYear		{ get { return settings.MonthsPerYear;		} }

	public DayTimeTypes CurrentTimeType = DayTimeTypes.Midnight;

	public bool TimePaused
	{ 
		get { return settings.TimePaused;	} 
		set { settings.TimePaused = value; }
	}


	// Use this for initialization
	void Awake()
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
			Debug.LogError( "Could not find WorldTimeSettings instance" );
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

		HourElapsed += HandleHourElapsed;
	}

	void OnLevelWasLoaded( int level )
	{
		GameObject sceneMaster = GameObject.Find( "SceneMaster" );
		
		if( null == sceneMaster )
		{
			Debug.LogError ( "Could not find SceneMaster instance" );
			return;
		}
		
		WorldTimeSettings nSettings = (WorldTimeSettings) sceneMaster.GetComponent( typeof(WorldTimeSettings) );

		nSettings.Clone( settings );
		settings = nSettings;
	}

	void HandleHourElapsed (uint _val)
	{
		if( settings.Day == m_PrevGameDay )
			return;

		if( settings.Hour >= GameDayLapseTime )
		{
			uint gameDays = 1 + (uint)(_val) / (uint)settings.HoursPerDay;
			settings.GameDay += gameDays;
			m_PrevGameDay = settings.Day;
			OnDelegate( GameDayElapsed, gameDays );
		}
	}

	public void AddGameDay( uint days = 1 )
	{
		settings.GameDay += days;
		m_PrevGameDay = settings.Day;
		OnDelegate( GameDayElapsed, days );
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

			UpdateMinutes();
			OnDelegate( MinuteElapsed, addMins );

			if( settings.DebugPrintType == WorldTimeSettings.DebugPrintTypes.OnMinute )
				PrintToConsole();
		}
	}

	void UpdateMinutes()
	{
		if( settings.Minute >= settings.MinutesPerHour )
		{
			uint minute = (uint)settings.Minute;
			uint hours = (uint)settings.Hour;
			uint addHours = minute / (uint)settings.MinutesPerHour;
			settings.Minute -= settings.MinutesPerHour * addHours;
			settings.Hour += addHours;

			UpdateHours();
			OnDelegate( HourElapsed, addHours );

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

			UpdateDays();
			OnDelegate( DayElapsed, addDays );
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
			
			UpdateWeeks();
			OnDelegate( WeekElapsed, addWeeks );
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
			
			UpdateMonths();
			OnDelegate( MonthElapsed, addMonths );
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
