using UnityEngine;
using System.Collections;

public class AnimRandStart : MonoBehaviour
{
	public bool m_RandomizeStart = true;
	public float m_MinSpeed = 1.0f;
	public float m_MaxSpeed = 1.0f;
	private float m_Speed = 1.0f;
	private bool firstUpdate = true;

	void Update()
	{
		if( firstUpdate )
		{
			if( m_RandomizeStart )
			{
				GetComponent<Animator>().speed = Random.value * 1000.0f;
			}

			m_Speed = Random.Range( m_MinSpeed, m_MaxSpeed );
			firstUpdate = false;
		}
		else
		{
			GetComponent<Animator>().speed = m_Speed;
		}
	}
}
