using UnityEngine;
using System.Collections;

public class Daylight : MonoBehaviour
{
	public Color MidnightColor = Color.black;
	public float MidnightIntensity = 0.2f;
	public float MidnightRange = 0.0f;
	public float MidnightSpotAngle = 0.0f;
	public Vector3 MidnightDirection = Vector3.down;

	public Color DawnColor = new Color(0.192f, 0.3098f, 0.4f);
	public float DawnIntensity = 0.2f;
	public float DawnRange = 0.0f;
	public float DawnSpotAngle = 0.0f;
	public Vector3 DawnDirection = Vector3.down;

	public Color SunriseColor = new Color(1.0f, 0.549019f, 0);
	public float SunriseIntensity = 0.2f;
	public float SunriseRange = 0.0f;
	public float SunriseSpotAngle = 0.0f;
	public Vector3 SunriseDirection = Vector3.down;

	public Color NoonColor = Color.yellow;
	public float NoonIntensity = 0.2f;
	public float NoonRange = 0.0f;
	public float NoonSpotAngle = 0.0f;
	public Vector3 NoonDirection = Vector3.down;

	public Color SunsetColor = new Color(1.0f, 0.2705f, 0.0f);
	public float SunsetIntensity = 0.2f;
	public float SunsetRange = 0.0f;
	public float SunsetSpotAngle = 0.0f;
	public Vector3 SunsetDirection = Vector3.down;

	public Color TwilightColor = new Color(0.28235f, 0.23921f, 0.545098f);
	public float TwilightIntensity = 0.2f;
	public float TwilightRange = 0.0f;
	public float TwilightSpotAngle = 0.0f;
	public Vector3 TwilightDirection = Vector3.down;

	public bool FlipMidnightRotationX = false;
	public bool FlipMidnightRotationY = false;
	public bool FlipMidnightRotationZ = false;
	public bool DebugPrintLerp = false;
	public bool DebugPrintTransition = false;
		
	protected DaylightTimeSetting Midnight = new DaylightTimeSetting();
	protected DaylightTimeSetting Dawn = new DaylightTimeSetting();
	protected DaylightTimeSetting Sunrise = new DaylightTimeSetting();
	protected DaylightTimeSetting Noon = new DaylightTimeSetting();
	protected DaylightTimeSetting Sunset = new DaylightTimeSetting();
	protected DaylightTimeSetting Twilight = new DaylightTimeSetting();
	protected DaylightTimeSetting TwilightWrapped = new DaylightTimeSetting();
	protected DaylightTimeSetting MidnightWrapped = new DaylightTimeSetting();

	protected DaylightTimeSetting FromSetting;
	protected DaylightTimeSetting ToSetting;
	protected WorldTime.DayTimeTypes DayTimeType = WorldTime.DayTimeTypes.None;
	protected float DayPercent = 0.0f;

	// Use this for initialization
	void Start()
	{
		WorldTime.Instance.DayTimeChanged += OnDayTimeChanged;
	}
	
	// Update is called once per frame
	void Update()
	{
		if( DayTimeType == WorldTime.DayTimeTypes.None )
			OnDayTimeChanged( WorldTime.Instance.CurrentTimeType );

		DayPercent = WorldTime.Instance.GetDayPercent();
		UpdateUserSettings();
		UpdateLight();
	}

	void UpdateUserSettings()
	{
		WorldTime time = WorldTime.Instance;

		Midnight.time = time.MidnightTime;
		Midnight.color = MidnightColor;
		Midnight.intensity = MidnightIntensity;
		Midnight.range = MidnightRange;
		Midnight.spotAngle = MidnightSpotAngle;
		Midnight.direction = MidnightDirection;
		
		Dawn.time = time.DawnTime;
		Dawn.color = DawnColor;
		Dawn.intensity = DawnIntensity;
		Dawn.range = DawnRange;
		Dawn.spotAngle = DawnSpotAngle;
		Dawn.direction = DawnDirection;
		
		Sunrise.time = time.SunriseTime;
		Sunrise.color = SunriseColor;
		Sunrise.intensity = SunriseIntensity;
		Sunrise.range = SunriseRange;
		Sunrise.spotAngle = SunriseSpotAngle;
		Sunrise.direction = SunriseDirection;
		
		Noon.time = time.NoonTime;
		Noon.color = NoonColor;
		Noon.intensity = NoonIntensity;
		Noon.range = NoonRange;
		Noon.spotAngle = NoonSpotAngle;
		Noon.direction = NoonDirection;
		
		Sunset.time = time.SunsetTime;
		Sunset.color = SunsetColor;
		Sunset.intensity = SunsetIntensity;
		Sunset.range = SunsetRange;
		Sunset.spotAngle = SunsetSpotAngle;
		Sunset.direction = SunsetDirection;
		
		Twilight.time = time.TwilightTime;
		Twilight.color = TwilightColor;
		Twilight.intensity = TwilightIntensity;
		Twilight.range = TwilightRange;
		Twilight.spotAngle = TwilightSpotAngle;
		Twilight.direction = TwilightDirection;
		
		MidnightWrapped = Midnight;
		MidnightWrapped.time += 1.0f;
		
		if( FlipMidnightRotationX )
			MidnightWrapped.direction.x += 360;
		
		if( FlipMidnightRotationY )
			MidnightWrapped.direction.y += 360;
		
		if( FlipMidnightRotationZ )
			MidnightWrapped.direction.z += 360;
		
		TwilightWrapped = LerpedSetting( Twilight, MidnightWrapped, 1.0f );
	}

	void OnDayChanged( uint _val )
	{
		if( WorldTime.Instance.CurrentTimeType == WorldTime.DayTimeTypes.Twilight )
		{
			FromSetting = TwilightWrapped;
			ToSetting = Midnight;
		}
	}

	void OnDayTimeChanged( WorldTime.DayTimeTypes type )
	{
		DayTimeType = WorldTime.Instance.CurrentTimeType;
		
		if( DebugPrintTransition )
			Console.Instance.addGameChatMessage( type.ToString() );

		// Tests from largest to least times
		// doing reverse order in this way is slightly faster
		switch( type )
		{
		case WorldTime.DayTimeTypes.Midnight:
			FromSetting = Midnight;
			ToSetting = Dawn;
			break;

		case WorldTime.DayTimeTypes.Dawn:
			FromSetting = Dawn;
			ToSetting = Sunrise;
			break;

		case WorldTime.DayTimeTypes.Sunrise:
			FromSetting = Sunrise;
			ToSetting = Noon;
			break;

		case WorldTime.DayTimeTypes.Noon:
			FromSetting = Noon;
			ToSetting = Sunset;
			break;

		case WorldTime.DayTimeTypes.Sunset:
			FromSetting = Sunset;
			ToSetting = Twilight;
			break;

		case WorldTime.DayTimeTypes.Twilight:

			if( DayPercent >= WorldTime.Instance.TwilightTime )
			{
				FromSetting = Twilight;
				ToSetting = MidnightWrapped;
			}
			else
			{
				FromSetting = TwilightWrapped;
				ToSetting = Midnight;
			}

			break;
		}
	}

	DaylightTimeSetting LerpedSetting( DaylightTimeSetting from, DaylightTimeSetting to, float t )
	{
		DaylightTimeSetting lerp = new DaylightTimeSetting();

		float absDelta = to.time - from.time;
		float absT = t - from.time;
		float perc = absT / absDelta;
		
		lerp.color = Color.Lerp( from.color, to.color, perc );
		lerp.intensity = Mathf.Lerp( from.intensity, to.intensity, perc );
		lerp.range = Mathf.Lerp( from.range, to.range, perc );
		lerp.spotAngle = Mathf.Lerp( from.range, to.range, perc );
		lerp.direction = Vector3.Lerp( from.direction, to.direction, perc );

		return lerp;
	}

	void UpdateLight()
	{
		DaylightTimeSetting lerp = LerpedSetting( FromSetting, ToSetting, DayPercent );

		light.color = lerp.color;
		light.intensity = lerp.intensity;
		light.range = lerp.range;
		light.spotAngle = lerp.spotAngle;
		transform.eulerAngles = lerp.direction;

		if( DebugPrintLerp )
			Debug.Log( "[Daylight] Time: " + DayPercent + " Color: " + light.color.ToString() + " Intensity: " + light.intensity + " Direction: " + lerp.direction.ToString() );
	}
}
