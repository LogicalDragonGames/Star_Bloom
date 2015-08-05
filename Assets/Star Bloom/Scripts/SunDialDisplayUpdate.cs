using UnityEngine;
using System.Collections;

public class SunDialDisplayUpdate : MonoBehaviour
{

	public Vector3 m_RotationOffset = new Vector3( 0.0f, 0.0f, -360.0f );
	void Start()
	{
		WorldTime.Instance.MinuteElapsed += OnMinute;
	}

	void OnDestroy()
	{
		if( null != WorldTime.Instance )
			WorldTime.Instance.MinuteElapsed -= OnMinute;
	}

	void OnMinute( uint val )
	{
		float dayPerc = WorldTime.Instance.GetDayPercent();

		Quaternion dRot = Quaternion.identity;
		dRot.eulerAngles = m_RotationOffset * dayPerc;

		transform.localRotation = dRot;
	}
}
