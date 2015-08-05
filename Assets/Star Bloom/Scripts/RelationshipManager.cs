using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelationshipManager : Singleton<RelationshipManager>
{
	private Dictionary<string, Relationship> m_RelationshipMap = new Dictionary<string, Relationship>();

	public Relationship GetRelationship( string name )
	{
		if( m_RelationshipMap.ContainsKey( name ) )
			return m_RelationshipMap[name];

		return new Relationship();
	}

	public void UpdateRelationship( string name, Relationship relationship )
	{
		m_RelationshipMap[name] = relationship;
	}

	public List<KeyValuePair<string, Relationship>> ToList()
	{
		List<KeyValuePair<string, Relationship>> outList = new List<KeyValuePair<string, Relationship>>();

		foreach( KeyValuePair<string, Relationship> entry in m_RelationshipMap )
		{
			outList.Add( entry );
		}

		return outList;
	}
}