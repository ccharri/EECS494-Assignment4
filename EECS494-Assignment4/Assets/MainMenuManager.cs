using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	private string userName;
	private bool showingNameWindow;
	private GameNetworkManager man;
	public GUISkin skin;

	// Use this for initialization
	void Start () {
		man = gameObject.GetComponent<GameNetworkManager>();
		userName = PlayerPrefs.GetString("userName", "Player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.skin = skin;
		if(man.enabled) return;

		GUILayout.BeginArea(new Rect((Screen.width - 600)/2, Screen.height/2, 600, Screen.height/2 - 50));
		GUILayout.BeginVertical("window");

		GUILayout.Label(userName, GUILayout.Height(50));

		if(GUILayout.Button ("Play", GUILayout.Height (50)))
		{
			man.enabled = true;
		}

		if(GUILayout.Button ("Change Name", GUILayout.Height (50)))
		{
			showingNameWindow = true;
		}

		if(showingNameWindow)
		{
			GUILayout.Window(0, new Rect(Screen.width/2 - 150, Screen.height/2 - 50, 300, 100), ChangeNameFunc, "", GUILayout.Width (300), GUILayout.Height(100));
		}

		if(GUILayout.Button ("Exit", GUILayout.Height (50)))
		{
			Application.Quit();
		}
		GUILayout.EndVertical();

		GUILayout.EndArea();
	}

	void ChangeNameFunc(int windowid)
	{
		GUILayout.BeginVertical();
		GUILayout.Label("Change Username", GUILayout.ExpandWidth(true));
		userName = GUILayout.TextArea(userName);
		if(GUILayout.Button ("Ok", GUILayout.ExpandWidth(true)))
		{
			showingNameWindow = false;
			PlayerPrefs.SetString("userName", userName);
			PlayerPrefs.Save();
		}
		GUILayout.EndVertical();
	}
}
