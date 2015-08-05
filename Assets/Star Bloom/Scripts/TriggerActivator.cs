using UnityEngine;
using System.Collections;

public class TriggerActivator : MonoBehaviour
{
	public enum ActivationTypes
	{
		ActiveOnTrigger = 0,
		InactiveOnTrigger
	};

	public bool StartDisabled = true;
	public ActivationTypes ActivationType;
	public string LocalComponent = "";
	private Light Target;

	// Use this for initialization
	void Start()
	{
		Target = this.GetComponent<Light>();

		if( null == Target )
		{
			Debug.LogError( "Could not locate local component '" + LocalComponent + "'!" );
			return;
		}

		if( StartDisabled )
			Target.enabled = false;
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnTriggerEnter( Collider obj )
	{
		if( null == Target )
			return;

		if( obj.tag == "Player" )
		{
			if( ActivationType == ActivationTypes.ActiveOnTrigger )
			{
				Target.enabled = true;
			}
			else if( ActivationType == ActivationTypes.InactiveOnTrigger )
			{
				Target.enabled = false;
			}
		}
	}

	void OnTriggerExit( Collider obj )
	{
		if( null == Target )
			return;

		if( obj.tag == "Player" )
		{
			if( ActivationType == ActivationTypes.ActiveOnTrigger )
			{
				Target.enabled = false;
			}
			else if( ActivationType == ActivationTypes.InactiveOnTrigger )
			{
				Target.enabled = true;
			}
		}
	}
}
