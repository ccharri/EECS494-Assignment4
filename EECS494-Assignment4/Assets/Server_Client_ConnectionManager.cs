using UnityEngine;
using System.Collections;

public class Server_Client_ConnectionManager : MonoBehaviour {
	private bool connected = false;
	private string serverIPText = "Server IP";
	private string serverPortText = "Server Port";
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if(!connected)
		{
			if(GUI.Button(new Rect(20, Screen.height - 100, 100, 80), "Host"))
			{
				NetworkConnectionError error;
				bool useNat = !Network.HavePublicAddress();
	        	error = Network.InitializeServer(2, 25000, useNat);
				if(error == NetworkConnectionError.NoError)
				{
					connected = true;
				}
				else {
					Debug.Log(error);
				}
			}
			
			serverIPText = GUI.TextField(new Rect(Screen.width - 200, Screen.height - 200, 200, 50), serverIPText);
			serverPortText = GUI.TextField (new Rect(Screen.width - 200, Screen.height - 150, 200, 50), serverPortText);
				
			if(GUI.Button(new Rect(Screen.width - 120, Screen.height - 100, 100, 80), "Connect"))
			{
				NetworkConnectionError error;
				error = Network.Connect (serverIPText, serverPortText);
				if(error == NetworkConnectionError.NoError)
				{
					connected = true;
				}
				else {
					// Check if it is the NATPunchthroughFailed error
					if(error == NetworkConnectionError.NATPunchthroughFailed)
					{
						OnFailedToConnect(error);
					}
					
					Debug.Log (error);
				}
			}
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
