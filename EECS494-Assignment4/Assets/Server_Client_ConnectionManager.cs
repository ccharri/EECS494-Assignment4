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
            int i = 0;
            while (i < hostData.Length) {
                Debug.Log("Game name: " + hostData[i].gameName);
                i++;
            }
            MasterServer.ClearHostList();
        }
	}

	void OnGUI() {
		if(!connected)
		{	
			GUILayout.BeginArea(new Rect(50, 50, Screen.width - 50, Screen.height - 50));
			
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
			
			GUILayout.EndVertical();
			
			Vector2 scrollPosition = new Vector2(0, 0);
			GUILayout.BeginScrollView(scrollPosition,GUILayout.Width(Screen.width - 100), GUILayout.Height(Screen.height - 100));
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
			GUILayout.EndArea();
//			serverIPText = GUI.TextField(new Rect(Screen.width - 500, Screen.height - 200, 200, 50), serverIPText);
//			serverPortText = GUI.TextField (new Rect(Screen.width - 500, Screen.height - 150, 200, 50), serverPortText);
//				
//			if(GUI.Button(new Rect(Screen.width - 520, Screen.height - 100, 100, 80), "Connect with IP"))
//			{	
//				NetworkConnectionError error;
//				error = Network.Connect (serverIPText, serverPortText);
//				if(error == NetworkConnectionError.NoError)
//				{
//					connected = true;
//				}
//				else {
//					Debug.Log (error);
//				}
//			}
//			
//			serverGUIDText = GUI.TextField (new Rect(Screen.width - 100, Screen.height - 150, 200, 50), serverGUIDText);
//			
//			if(GUI.Button (new Rect(Screen.width - 200, Screen.height-100, 200, 50), "Connect with GUID"))
//			{
//				NetworkConnectionError error;
//				error = Network.Connect(serverGUIDText);
//				if(error == NetworkConnectionError.NoError)
//				{
//					connected = true;
//				}
//				else {
//					Debug.Log (error);
//				}
//			}
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
