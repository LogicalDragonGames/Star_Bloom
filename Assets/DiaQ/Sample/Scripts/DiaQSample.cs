using UnityEngine;
using System.Collections;
using DiaQ;

// 
// This is a simple script to show to use the DiaQ runtime API
// There are three cubes in the scene. The player is expected to click on the first one to start the conversation with the cube
// This wil lstart the graph named "Sample" and the player will, through it, receive quests to click on the other blocks
// When he clicks on the other cubes this script will simply tell DiaQ to update the quest conditions
//

public class DiaQSample : MonoBehaviour 
{
	private const string SampleGraphName = "Sample";
	private Camera cam;
	private bool inDialogue = false;
	private bool showQuestList = false;
	private Rect winRect = new Rect(0, 0, 200, 350);
	private Rect questListRect = new Rect(200, 0, 200, 350);
	private DiaQConversation data = null;
	private int selectedQuest = -1;

	void Awake() 
	{
		cam = GetComponent<Camera>();
		
		// tell DiaQ to load its assets
		if (false == DiaQEngine.Instance.Load())
		{
			enabled = false; // failed, so disable since can't go on with demo
		}
	}
	
	void Update() 
	{
		if (false == inDialogue)
		{
			// mouse click?
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{	
				// check what was clicked on
				RaycastHit hit;
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit))
				{
					if (hit.transform.name.Equals("Cube_1")) Clicked_On_Cube1();
					else if (hit.transform.name.Equals("Cube_2")) Clicked_On_Cube2();
					else if (hit.transform.name.Equals("Cube_3")) Clicked_On_Cube3();
				}
			}
		}
	}

	private void Clicked_On_Cube1()
	{
		data = DiaQEngine.Instance.StartGraph(SampleGraphName, false);

		if (data != null)
		{	// there was data, meaning there is something to show to the payer in the GUI
			inDialogue = true;

			winRect.x = Screen.width * 0.5f - winRect.width * 0.5f;
			winRect.y = Screen.height * 0.5f - winRect.height * 0.5f;
		}
		else
		{
			Debug.LogWarning("There was no DiaQData (conversation) to show.");
		}
	}

	private void Clicked_On_Cube2()
	{
		// Block 2 was clicked on once. Update any accepted quest that might 
		// be using a condition named, Block2
		DiaQEngine.Instance.UpdateQuestConditions("Block2", 1);
	}

	private void Clicked_On_Cube3()
	{
		// Block 3 was clicked on once. Update any accepted quest that might 
		// be using a condition named, Block3
		DiaQEngine.Instance.UpdateQuestConditions("Block3", 1);
	}

	void OnGUI()
	{
		if (inDialogue)
		{
			winRect = GUI.Window(1, winRect, DialogueWindow, GUIContent.none);
			return;
		}

		if (showQuestList)
		{
			questListRect = GUI.Window(2, questListRect, QuestListWindow, "Quest List");
		}
		else
		{
			if (GUI.Button(new Rect(Screen.width - 200, 10, 100, 20), "Quest List"))
			{
				questListRect.x = Screen.width - questListRect.width - 10;
				questListRect.y = 10;
				showQuestList = true;
			}
		}
	}

	void DialogueWindow(int id)
	{
		// Show the Conversation Text (what NPC is saying to player)
		if (data.conversationText != null)
		{
			GUILayout.Label(data.conversationText);
		}
		GUILayout.Space(15);

		// Show the choices the Player has
		if (data.choices != null)
		{
			for (int i = 0; i < data.choices.Length; i++)
			{
				if (string.IsNullOrEmpty(data.choices[i]))
				{
					// no text that can be shown, so ignore this choice
					continue;
				}

				if (GUILayout.Button(data.choices[i]))
				{
					// Tell DiaQ what choice the player made and get new data that was generated because of the choice
					data = DiaQEngine.Instance.SubmitChoice(i);

					// if NULL was pased back then the Graph/Dialogue is either over or ane rror occured
					if (data == null)
					{
						inDialogue = false;
						return;
					}
				}
			}
		}

		GUI.DragWindow();
	}

	void QuestListWindow(int id)
	{
		for (int i = 0; i < DiaQEngine.Instance.acceptedQuests.Count; i++)
		{
			if (GUILayout.Toggle(selectedQuest == i, DiaQEngine.Instance.acceptedQuests[i].name, GUI.skin.button)) selectedQuest = i;
		}

		if (selectedQuest >= 0)
		{
			GUILayout.Space(15);
			GUILayout.Label("Quest: " + DiaQEngine.Instance.acceptedQuests[selectedQuest].name);
			GUILayout.Label("Quest is " + (DiaQEngine.Instance.acceptedQuests[selectedQuest].IsCompleted ? "Completed" : "In Progress"));
			GUILayout.Label(DiaQEngine.Instance.acceptedQuests[selectedQuest].text);
		}

		GUI.DragWindow();
	}

	void TEST()
	{
		Debug.Log("TEST");
	}
}
