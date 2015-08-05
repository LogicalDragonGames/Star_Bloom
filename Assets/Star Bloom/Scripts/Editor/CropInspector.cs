using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Crop))]
public class CropInspector : Editor
{
	public override void OnInspectorGUI()
	{
		//Called whenever the inspector is drawn for this object.
		DrawDefaultInspector();
		
		Crop script = (Crop)target;

		int lifespan = script.TotalStagesDuration;
		if( lifespan > 0 ) lifespan -= 1; // Handle 0th day, this value is the total number of days

		script.GameDayAge = EditorGUILayout.IntSlider( "Game Day Age", script.GameDayAge, 0, lifespan );
		script.m_HarvestYield = EditorGUILayout.IntSlider( "Harvest Yield", script.m_HarvestYield, 0, 20 );
	}
}