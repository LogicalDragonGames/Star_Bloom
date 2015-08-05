using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FarmTileBinding))]
public class FarmTileBindingInspector : Editor
{
	public override void OnInspectorGUI()
	{
		//Called whenever the inspector is drawn for this object.
		DrawDefaultInspector();

		FarmTileBinding script = (FarmTileBinding)target;

		script.IsWatered = EditorGUILayout.Toggle( "Is Watered", script.IsWatered );

		script.Crop = (UnityEngine.GameObject)EditorGUILayout.ObjectField( "Crop", script.Crop, typeof(UnityEngine.GameObject), false );

		//This draws the default screen.  You don't need this if you want
		//to start from scratch, but I use this when I'm just adding a button or
		//some small addition and don't feel like recreating the whole inspector.
		script.AmountFertilized = EditorGUILayout.Slider( "Amount Fertilized", script.AmountFertilized, 0.0f, 1.0f );
	}
}