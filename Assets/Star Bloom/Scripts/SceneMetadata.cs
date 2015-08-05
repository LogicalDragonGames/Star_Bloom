using UnityEngine;
using System.Collections;

public class SceneMetadata : Singleton<SceneMetadata>
{
	public int LevelsLoaded = 0;
	public bool HasSceneLoadPlayerTransform = false;
	public Vector3 SceneLoadPlayerPosition = Vector3.zero;
	public Vector3 SceneLoadPlayerRotation = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		DontDestroyOnLoad( transform.gameObject );
	}

	GameObject GetPlayer()
	{
		return GameObject.Find( "Player" );
	}

	public void OnLevelWasLoaded( int level )
	{
		++LevelsLoaded;

		if( HasSceneLoadPlayerTransform )
			SetPlayerPosition();
	}

	public void SetPlayerPosition()
	{
		GameObject player = GetPlayer();

		if( null == player )
		{
			Debug.Log( "Failed to find player object." );
			return;
		}

		player.transform.position = SceneLoadPlayerPosition;
		player.transform.eulerAngles = SceneLoadPlayerRotation;
	}
}
