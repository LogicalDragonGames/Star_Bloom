using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
	public enum Transition
	{
		None = 0,
		FadeOut
	}

	public string SceneName = "";
	public Transition TransitionType = Transition.FadeOut;
	public float TransitionTime = 1.0f;
	public float PostTransitionBuffer = 0.2f;
	private float m_RemainingTime = 0.0f;
	private bool m_IsTransitioning = false;

	public Color m_CurrentScreenOverlayColor = new Color(0,0,0,0);	// default starting color: black and fully transparrent
	public Color m_TargetScreenOverlayColor = new Color(0,0,0,0);	// default target color: black and fully transparrent
	public Color m_DeltaColor = new Color(0,0,0,0);					// the delta-color is basically the "speed / second" at which the current color should change
	private GUIStyle m_BackgroundStyle = new GUIStyle();			// Style for background tiling
	private Texture2D m_FadeTexture = null;							// 1x1 pixel texture used for fading
	private int m_FadeGUIDepth = -1000;								// make sure this texture is drawn on top of everything

	public bool m_HasLoadTransform = true;
	public Vector3 m_LoadPosition = Vector3.zero;
	public Vector3 m_LoadRotation = Vector3.zero;

	private void Awake()
	{
		m_FadeTexture = new Texture2D(1, 1);        
		m_BackgroundStyle.normal.background = m_FadeTexture;
		SetScreenOverlayColor(m_CurrentScreenOverlayColor);
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		// Do nothing until transitioning
		if( !m_IsTransitioning )
			return;

		// Transition to the next scene
		// I do this first to allow the transition to finish
		// before ending the scene
		if( m_RemainingTime + PostTransitionBuffer <= 0.0f )
		{
			SceneMetadata.Instance.HasSceneLoadPlayerTransform = m_HasLoadTransform;

			if( m_HasLoadTransform )
			{
				SceneMetadata.Instance.SceneLoadPlayerPosition = m_LoadPosition;
				SceneMetadata.Instance.SceneLoadPlayerRotation = m_LoadRotation;
			}

			SceneMetadata.Instance.PrepareTransition();

			WorldTime.Instance.TimePaused = false;
			Application.LoadLevel( SceneName );
		}

		// Decrement the timer
		m_RemainingTime -= Time.deltaTime;

		// Transition
		switch( TransitionType )
		{
		case Transition.None:
			break;

		case Transition.FadeOut:
			DoFadeOut();
			break;

		default:
			Debug.LogWarning( "Unrecognized transition type: " + TransitionType );
			break;
		}
	}

	void OnTriggerEnter( Collider other ) 
	{
		if( other.collider.tag == "Player" ) //Checks if the Player is inside the trigger
		{
			DoTransition();
		}
	}

	public void DoTransition()
	{
		m_IsTransitioning = true;
		WorldTime.Instance.TimePaused = true;
		m_RemainingTime = TransitionTime;
	}

	float PercentCompletion()
	{
		if( TransitionTime >= 0.0f )
			return m_RemainingTime / TransitionTime;
	
		else
			return 0.0f;
	}


	//
	//
	// FadeOut Region
	//
	//

	// draw the texture and perform the fade:
	private void OnGUI()
	{   
		if( !m_IsTransitioning )
			return;

		// if the current color of the screen is not equal to the desired color: keep fading!
		if (m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor)
		{			
			// if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
			if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * Time.deltaTime)
			{
				m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
				SetScreenOverlayColor(m_CurrentScreenOverlayColor);
				m_DeltaColor = new Color(0,0,0,0);
			}
			else
			{
				// fade!
				SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime);
			}
		}
		
		// only draw the texture when the alpha value is greater than 0:
		if (m_CurrentScreenOverlayColor.a > 0)
		{			
			GUI.depth = m_FadeGUIDepth;
			GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
		}
	}
	
	
	// instantly set the current color of the screen-texture to "newScreenOverlayColor"
	// can be usefull if you want to start a scene fully black and then fade to opague
	public void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		m_CurrentScreenOverlayColor = newScreenOverlayColor;
		m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
		m_FadeTexture.Apply();
	}
	
	
	// initiate a fade from the current screen color (set using "SetScreenOverlayColor") towards "newScreenOverlayColor" taking "fadeDuration" seconds
	public void StartFade(Color newScreenOverlayColor, float fadeDuration)
	{
		if (fadeDuration <= 0.0f)		// can't have a fade last -2455.05 seconds!
		{
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else							// initiate the fade: set the target-color and the delta-color
		{
			m_TargetScreenOverlayColor = newScreenOverlayColor;
			m_DeltaColor = (m_TargetScreenOverlayColor - m_CurrentScreenOverlayColor) / fadeDuration;
		}
	}

	void DoFadeOut()
	{
		StartFade( m_TargetScreenOverlayColor, TransitionTime );
		/*if( m_FadeTexture == null )
			return;

		GUI.color.a = PercentCompletion();
		GUI.depth = -1000;
		
		GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), fadeTexture);*/
	}
}
