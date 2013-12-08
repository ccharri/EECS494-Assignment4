using UnityEngine;
using System.Collections;

public class GameNetworkManager : MonoBehaviour {
	private bool connected = false;
	private string serverIPText = "Server IP";
	private string serverPortText = "Server Port";
	private string serverGUIDText = "Server GUID";
	private HostData[] hostData;
	private string gameName = "Game Name";

	private bool raceListShow = false;
	private int raceListEntry = 0;
	private GUIContent[] raceList;
	private GUIStyle raceListStyle;
	private bool racePicked = false;

	public GUISkin skin;
	
	void Awake() {
		Refresh ();
		NameDatabase.clearNames();
    }
	
	// Use this for initialization
	void Start () {
		// Make some content for the popup list
		raceList = new GUIContent[2];
		raceList[0] = new GUIContent("Undead");
		raceList[1] = new GUIContent("Arcane");
	 
		// Make a GUIStyle that has a solid white hover/onHover background to indicate highlighted items
		raceListStyle = new GUIStyle();
		raceListStyle.normal.textColor = Color.white;
		var tex = new Texture2D(2, 2);
		var colors = new Color[4];

		for (int i = 0; i < 4; i++)
			colors[i] = Color.white;
		//foreach (Color temp in colors) 
		//	temp = Color.white;

		tex.SetPixels(colors);
		tex.Apply();
		raceListStyle.hover.background = tex;
		raceListStyle.onHover.background = tex;
		raceListStyle.padding.left = raceListStyle.padding.right = raceListStyle.padding.top = raceListStyle.padding.bottom = 4;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Refresh()
	{
		MasterServer.ClearHostList();
		MasterServer.RequestHostList("BlitzTD");
		hostData = MasterServer.PollHostList();
	}
	
	void OnServerInitialized () 
	{
		Debug.Log("Server Initialized");
		connected = true;
		NameDatabase.clearNames();
		NameDatabase.addName(Network.player.guid, PlayerPrefs.GetString("userName"));
	}
 
	void OnConnectedToServer () 
	{
		NameDatabase.clearNames();
		Debug.Log ("Connected to Server");
		connected = true;
		string myname = PlayerPrefs.GetString("userName");
		NameDatabase.addName(Network.player.guid, myname);
		networkView.RPC ("registerName", RPCMode.Server, myname, Network.player.guid);
		networkView.RPC ("requestNames", RPCMode.Server);
	}

	void OnMasterServerEvent(MasterServerEvent mse)
	{
		if (mse == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Registered Server");
		}
		else if(mse == MasterServerEvent.HostListReceived)
		{
			Debug.Log ("Received Host List");
		}
		else
		{
			Debug.Log (mse);
			connected = false;
		}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{

	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		NameDatabase.removeName(player.guid);
		networkView.RPC ("removeName", RPCMode.Others, player.guid);
	}

	void OnFailedToConnect(NetworkConnectionError error) {
		connected = false;
        Debug.Log("Could not connect to server: " + error);
    }

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		connected = false;
		if(Network.isServer)
		{
			Debug.Log("Local server connection disconnected");
		}
		else
		{
			if(info == NetworkDisconnection.LostConnection)
			{
				Debug.Log ("Lost connection to the server.");
			}
			else
			{
				Debug.Log ("Successfully disconnected from the server.");
			}
		}
	}

	void OnGUI() {
		GUI.skin = skin;

		if(!connected)
		{	
			OnGUI_Unconnected();
		}
		else
		{
			OnGUI_Connected();
		}
	}

	void OnGUI_Unconnected()
	{
		GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical("box",  GUILayout.ExpandWidth(false), GUILayout.Width (200));

		gameName = GUILayout.TextField (gameName,  GUILayout.ExpandWidth(false), GUILayout.Width (200), GUILayout.Height (50));

		if(GUILayout.Button("Host", GUILayout.ExpandWidth(true), GUILayout.Height(100)))
		{
			NetworkConnectionError error;
			bool useNat = !Network.HavePublicAddress();
			Debug.Log ("Use Nat:" + useNat);
			error = Network.InitializeServer(1, 25000, useNat);
			if(error == NetworkConnectionError.NoError)
			{
				connected = true;
				MasterServer.RegisterHost("BlitzTD", gameName);
			}
			else {
				Debug.Log(error);
			}
		}
		
		serverIPText = GUILayout.TextField(serverIPText, GUILayout.ExpandWidth(false),GUILayout.Width (200), GUILayout.Height(50));
		serverPortText = GUILayout.TextField ( serverPortText, GUILayout.ExpandWidth(false),GUILayout.Width (200),GUILayout.Height(50));
		
		if(GUILayout.Button("Connect with IP", GUILayout.ExpandWidth(true), GUILayout.Height(100)))
		{	
			NetworkConnectionError error;
			error = Network.Connect (serverIPText, int.Parse(serverPortText));
			if(error == NetworkConnectionError.NoError)
			{
				connected = true;
			}
			else {
				Debug.Log (error);
			}
		}
		
		serverGUIDText = GUILayout.TextField (serverGUIDText, GUILayout.ExpandWidth(false),GUILayout.Width (200), GUILayout.Height(50));
		
		if(GUILayout.Button ( "Connect with GUID", GUILayout.ExpandWidth(true), GUILayout.Height(100)))
		{
			NetworkConnectionError error;
			error = Network.Connect(serverGUIDText);
			if(error == NetworkConnectionError.NoError)
			{
				connected = true;
			}
			else {
				Debug.Log (error);
			}
		}
		
		GUILayout.EndVertical();

		GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		Vector2 scrollPosition = new Vector2(0, 0);
		GUILayout.BeginScrollView(scrollPosition, "box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.Label("Game List");
		
		HostData[] data = hostData = MasterServer.PollHostList();
		// Go through all the hosts in the host list
		for(int i = 0 ; i < data.Length; i++)
		{
			HostData element = data[i];
			
			GUILayout.BeginHorizontal();	
			string name = element.gameName;
			GUILayout.Label(name);	
			GUILayout.FlexibleSpace();
			GUILayout.Label (element.connectedPlayers + " / " + element.playerLimit);
			GUILayout.FlexibleSpace();
			string hostInfo = "[";
			string[] info = element.ip;
			for (int j = 0; j < info.Length; j++)
				hostInfo = hostInfo + info[j] + ":" + element.port + " ";
			hostInfo = hostInfo + "]";
			GUILayout.Label(hostInfo);	
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Connect"))
			{
				// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
				Network.Connect(element);			
			}
			GUILayout.EndHorizontal();	
		}
		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		
		if(GUILayout.Button("Refresh"))
		{
			Refresh ();
		}
		
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button ("Back"))
		{
			this.enabled = false;
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	void OnGUI_Connected()
	{
		if(GUI.Button (new Rect(Screen.width - 300, Screen.height-100, 200, 50), "Disconnect"))
		{
			Network.Disconnect();
			connected = false;
		}

		if(Network.isServer)
		{
			if(Network.HavePublicAddress())
			{
				NetworkPlayer thisPlayer = Network.player;
				
				GUI.Label (new Rect(20,20, 125, 50), "Public IP Address");
				GUI.Label (new Rect(20,70,125,30), Network.player.ipAddress + ":" + Network.player.port);
				//GUI.Label(new Rect(20, 70, 100, 30), Network.proxyIP + ":" + Network.proxyPort);
				
				GUI.Label (new Rect(20, 120, 1250, 30), Network.player.guid);
			}
			else
			{
				GUI.Label (new Rect(20,20, 125, 50), "NAT Facilitator IP");
				GUI.Label(new Rect(20, 70, 125, 30), Network.natFacilitatorIP + ":" + Network.natFacilitatorPort);
			}

			if(GUI.Button (new Rect((Screen.width - 100)/2, Screen.height - 300, 200, 200), "Start"))
			{
				MasterServer.UnregisterHost();
				networkView.RPC ("launchGameScene", RPCMode.AllBuffered);
			}

			OnGUI_DisplayPlayers();
		}
		else
		{
			OnGUI_DisplayPlayers();
		}
	}

	void OnGUI_DisplayPlayers()
	{
		Rect playerArea = new Rect(200, 200, Screen.width - 300, (Screen.height/2) - 100);
		GUILayout.BeginArea(playerArea);
		GUILayout.BeginVertical();

		OnGUI_DisplayPlayer(Network.player);

		foreach(NetworkPlayer player in Network.connections)
		{
			OnGUI_DisplayPlayer(player);
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	void OnGUI_DisplayPlayer(NetworkPlayer player)
	{
		GUILayout.BeginHorizontal(GUILayout.Height(50));

		GUILayout.Label(NameDatabase.getName(player.guid), GUILayout.Width(200));
		GUILayout.FlexibleSpace();

    //if (Popup.List(new Rect(50, 100, 100, 20), ref raceListShow, ref raceListEntry, new GUIContent("Click me!"), raceList, raceListStyle)) {
    //  racePicked = true;
    //}

		GUILayout.FlexibleSpace();

		GUILayout.Label (Network.GetAveragePing(player).ToString(), GUILayout.Width(50));
		GUILayout.FlexibleSpace();
		if(Network.isServer && player != Network.player)
		{
			if(GUILayout.Button ("Kick", GUILayout.Width(50), GUILayout.Height(50)))
			{
				Network.CloseConnection(player, true);
			}
		}
		
		GUILayout.EndHorizontal();
	}

	[RPC]
	void launchGameScene(NetworkMessageInfo info)
	{
		Debug.Log ("launchGameScene RPC Call received from " + info);
		Application.LoadLevel("GameScene");
	}

	[RPC]
	void requestNames(NetworkMessageInfo info)
	{
		Debug.Log ("requestNames received from " + info);

		foreach(string key in NameDatabase.getKeys())
		{
			//Register other users
			Debug.Log ("Sent name= " + NameDatabase.getName(key) + ", key = " + key);
			if(Network.isServer && key == Network.player.guid)
			{
				networkView.RPC("registerName", info.sender, NameDatabase.getName(key), "");
			}
			else
			{
				networkView.RPC ("registerName", info.sender, NameDatabase.getName(key), key);
			}
		}
	}

	[RPC]
	void registerName(string name, string key, NetworkMessageInfo info)
	{
		Debug.Log ("registerName RPC Call received from " + info +", name = " + name + ", key = " + key);
		NameDatabase.addName(key, name);
	}

	[RPC]
	void removeName(string key, NetworkMessageInfo info)
	{
		Debug.Log ("removeName RPC Call received from " + info + ", key = " + key);
		NameDatabase.removeName(key);
	}

}
