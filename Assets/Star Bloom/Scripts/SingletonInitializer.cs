using UnityEngine;
using System.Collections;
using DiaQ;

public class SingletonInitializer : MonoBehaviour
{
	void Awake()
	{
		DiaQEngine.Instance.Load();
	}

	// Use this for initialization
	void Start()
	{
		MonoBehaviour obj = SceneMetadata.Instance;
		obj = Console.Instance;
		obj = WorldTime.Instance;

		// Tricking C# into not throwing a warning
		// Assigning an instance to this object forces instantiation
		if( obj )
			obj = null;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
