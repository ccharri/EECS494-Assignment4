using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	private string userName;
	private bool showingNameWindow;
	private GameNetworkManager man;

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
		if(man.enabled) return;

		GUILayout.BeginArea(new Rect((Screen.width - 300)/2, Screen.height/2, 600, Screen.height/2 - 50));

		GUILayout.TextField(userName, GUILayout.Height(50));

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
			GUILayout.Window(0, new Rect(Screen.width/2 - 100, Screen.height/2 - 50, 200, 100), ChangeNameFunc, "Enter Username", GUILayout.Width (200), GUILayout.Height(100));
		}

		GUILayout.EndArea();
	}

	void ChangeNameFunc(int windowid)
	{
		userName = GUILayout.TextArea(userName);
		if(GUILayout.Button ("Ok", GUILayout.Width (50)))
		{
			showingNameWindow = false;
			PlayerPrefs.SetString("userName", userName);
			PlayerPrefs.Save();
		}
	}
}
