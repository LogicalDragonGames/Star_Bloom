using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Relationship
{
	public RelationshipCategory[] m_Categories = {};
	public int m_CurrentCategory = 0;
	public float m_CurrentProgress = 0f;

	public void AddProgress( float add )
	{
		if( add == 0.0f )
			return;

		if( add > 0.0f )
			hAddPositiveProgress( add );
		else
			hAddNegativeProgress( add );
	}

	void hAddPositiveProgress( float add )
	{
		// Loop through each category, decrementing as we go
		// Has to be N cycles, as category sizes are non-uniform
		while( add >= 0f )
		{
			float catProgressMax = m_Categories[m_CurrentCategory].m_MaxProgress;
			
			// Simplest case, all we have to do is add our progress and exit
			if( add+m_CurrentProgress <= catProgressMax )
			{
				m_CurrentProgress += add;
				return;
			}
			
			// Adding a value beyond this category's max bounds
			else
			{
				// Is this the final category?
				if( m_CurrentCategory == m_Categories.Length-1 )
				{
					// Clamp our max values and exit
					m_CurrentProgress = catProgressMax;
					return;
				}
				
				// Increment to the next category and take the difference
				// from our added value
				else
				{
					add -= (catProgressMax - m_CurrentProgress);
					m_CurrentProgress = 0f;
					++m_CurrentCategory;
				}
			}
		}
	}

	void hAddNegativeProgress( float add )
	{
		// Loop through each category, incrementing as we go
		// Has to be N cycles, as category sizes are non-uniform
		while( add <= 0f )
		{	
			// Simplest case, all we have to do is sub our progress and exit
			if( m_CurrentProgress >= add )
			{
				m_CurrentProgress -= add;
				return;
			}
			
			// Subbing a value below zero
			else
			{
				// Is this the final category?
				if( m_CurrentCategory == 0 )
				{
					// Clamp our min value and exit
					m_CurrentProgress = 0.0f;
					return;
				}
				
				// Decrement to the next category and take the difference
				// from our added value
				else
				{
					add += m_CurrentProgress;
					--m_CurrentCategory;
					m_CurrentProgress = m_Categories[m_CurrentCategory].m_MaxProgress;
				}
			}
		}
	}
}