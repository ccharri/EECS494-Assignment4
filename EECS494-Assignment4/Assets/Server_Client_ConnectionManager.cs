using UnityEngine;
using System.Collections;

public class Server_Client_ConnectionManager : MonoBehaviour {
	private bool connected = false;
	private string serverIPText = "Server IP";
	private string serverPortText = "Server Port";
	private string serverGUIDText = "Server GUID";
	
	void Awake() {
        MasterServer.ClearHostList();
        MasterServer.RequestHostList("BlitzTD");
    }
	
	// Use this for initialization
	void Start () {
		Network.logLevel = UnityEngine.NetworkLogLevel.Full;
	}
	
	// Update is called once per frame
	void Update () {
		if (MasterServer.PollHostList().Length != 0) {
            HostData[] hostData = MasterServer.PollHostList();
           // int i = 0;
           // while (i < hostData.Length) {
           //     Debug.Log("Game name: " + hostData[i].gameName);
           //     i++;
           // }
           // MasterServer.ClearHostList(); //Wat.  Why is this here ?!
        }
	}

	void OnGUI() {
		if(!connected)
		{	
			GUILayout.BeginArea(new Rect(50, 50, Screen.width - 50, Screen.height - 50));
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			if(GUILayout.Button("Host", GUILayout.Width(100), GUILayout.Height(100)))
			{
				NetworkConnectionError error;
				bool useNat = !Network.HavePublicAddress();
				Debug.Log ("Use Nat:" + useNat);
	        	error = Network.InitializeServer(2, 25000, useNat);
				if(error == NetworkConnectionError.NoError)
				{
					connected = true;
					MasterServer.RegisterHost("BlitzTD", "This is my test game");
				}
				else {
					Debug.Log(error);
				}
			}
			
			serverIPText = GUILayout.TextField(serverIPText, GUILayout.Width(200), GUILayout.Height(50));
			serverPortText = GUILayout.TextField ( serverPortText, GUILayout.Width(200), GUILayout.Height(50));
				
			if(GUILayout.Button("Connect with IP", GUILayout.Width(200), GUILayout.Height(100)))
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
			
			serverGUIDText = GUILayout.TextField (serverGUIDText, GUILayout.Width(200), GUILayout.Height(50));
			
			if(GUILayout.Button ( "Connect with GUID", GUILayout.Width(200), GUILayout.Height(100)))
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
			
			Vector2 scrollPosition = new Vector2(0, 0);
			GUILayout.BeginScrollView(scrollPosition,GUILayout.Width(Screen.width - 400), GUILayout.Height(Screen.height - 300));
			GUILayout.Label("Game List");
			
			HostData[] data = MasterServer.PollHostList();
			// Go through all the hosts in the host list
			for(int i = 0 ; i < data.Length; i++)
			{
				HostData element = data[i];
				
				GUILayout.BeginHorizontal();	
				string name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
				GUILayout.Label(name);	
				GUILayout.Space(5);
				string hostInfo = "[";
				string[] info = element.ip;
				for (int j = 0; j < info.Length; j++)
					hostInfo = hostInfo + info[j] + ":" + element.port + " ";
				hostInfo = hostInfo + "]";
				GUILayout.Label(hostInfo);	
				GUILayout.Space(5);
				GUILayout.Label(element.comment);
				GUILayout.Space(5);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Connect"))
				{
					// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
					Network.Connect(element);			
				}
				GUILayout.EndHorizontal();	
			}
			GUILayout.EndScrollView();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		else
		{
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
				
				int i = 0;
        		while (i < Network.connections.Length) {
            		GUI.Label(new Rect(500, 50 + 50*i, 125, 50) ,"Player " + Network.connections[i] + " - " + Network.GetAveragePing(Network.connections[i]) + " ms");
           			i++;
        		}
			}
			else
			{
				int i = 0;
        		while (i < Network.connections.Length) {
            		GUI.Label(new Rect(500, 50 + 50*i, 125, 50), "Player " + Network.connections[i] + " - " + Network.GetAveragePing(Network.connections[i]) + " ms");
            		i++;
        		}
			}
		}
	}
}
