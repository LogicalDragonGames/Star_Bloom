using UnityEngine;
using System.Collections;
using System.Text;

public class ClockTimeDisplay : MonoBehaviour
{
	protected dfLabel m_Label;

	// Use this for initialization
	void Start()
	{
		WorldTime.Instance.MinuteElapsed += OnMinute;

		m_Label = GetComponent<dfLabel>();

		if( null == m_Label )
			Debug.LogError( "Failed to find 'dfLabel' attached to this object" );
	}

	void OnDestroy()
	{
		if( null != WorldTime.Instance )
			WorldTime.Instance.MinuteElapsed -= OnMinute;
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	public void OnMinute( uint val )
	{
		float hour = WorldTime.Instance.Hour+1;
		float minute = WorldTime.Instance.Minute;

		StringBuilder sb = new StringBuilder( string.Format( ":{0,2:00}", minute ) );
		sb[ sb.Length-1 ] = '0';

		string timeStr = "";
		string minuteStr = sb.ToString();

		// Hour-AM/PM
		if( hour < 12 )
			timeStr += hour.ToString() + minuteStr + " AM";
		else if( hour == 12 )
			timeStr += hour.ToString() + minuteStr + " PM";
		else
			timeStr += (hour-12).ToString() + minuteStr + " PM";

		timeStr += System.Environment.NewLine;

		float day = WorldTime.Instance.Day;
		float dayOfMonth = day + WorldTime.Instance.Week * WorldTime.Instance.DaysPerWeek + 1;

		// Day Month, Year
		switch( (int)day )
		{
		case 0:
			timeStr += "Mon ";
			break;
		case 1:
			timeStr += "Tue ";
			break;
		case 2:
			timeStr += "Wed ";
			break;
		case 3:
			timeStr += "Thu ";
			break;
		case 4:
			timeStr += "Fri ";
			break;
		case 5:
			timeStr += "Sat ";
			break;
		case 6:
			timeStr += "Sun ";
			break;
		}

		timeStr += dayOfMonth.ToString();
		timeStr += "[/color]";

		m_Label.Text = timeStr;
	}
}
