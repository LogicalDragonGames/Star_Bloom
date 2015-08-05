using UnityEngine;
using System.Collections;

public class ConsoleSettings : MonoBehaviour
{

	public bool usingChat = false;	// Can be used to determine if we need to stop player movement since we're chatting
	public bool showChat = true;	// Show/Hide the chat
	public GUISkin skin;            // Skin
	public int width = 190;
	public int height = 180;
	public int x = 0;
	public int y = 0;
	public uint maxChatLines = 20;
	public bool centerHorizontal = false;
	public bool centerVertical = false;
	public bool clampRight = true;
	public bool clampBottom = true;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
