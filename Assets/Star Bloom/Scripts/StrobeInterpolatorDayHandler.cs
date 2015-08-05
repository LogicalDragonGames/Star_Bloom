using UnityEngine;
using System.Collections;

public class StrobeInterpolatorDayHandler : MonoBehaviour
{
	public WorldTime.DayTimeTypes[] ActivateOnTimes = { WorldTime.DayTimeTypes.Midnight };

	protected StrobeLightInterpolator strobeInterpolator = null;

	// Use this for initialization
	void Start()
	{
		WorldTime.Instance.DayTimeChanged += DayTimeChanged;
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void DayTimeChanged( WorldTime.DayTimeTypes timeType )
	{
		bool activateSI = false;
		foreach( WorldTime.DayTimeTypes type in ActivateOnTimes )
			if( type == timeType )
			{
				activateSI = true;
				break;
			}

		if( activateSI )
		{
			strobeInterpolator = GetComponent<StrobeLightInterpolator>();

			if( strobeInterpolator )
			{
				strobeInterpolator.Restart();
			}
		}
	}
}
