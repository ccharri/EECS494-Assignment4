using UnityEngine;
using System.Collections;

public class Server_Client_ConnectionManager : MonoBehaviour {
	private bool connected = false;
	
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
			
			var IP = GUI.TextField(new Rect(Screen.width - 120, Screen.height - 200, 50, 100), "Server IP");
			var Port = GUI.TextField (new Rect(Screen.width - 120, Screen.height - 150, 50, 100), "Server Port");
				
			if(GUI.Button(new Rect(Screen.width - 120, Screen.height - 100, 100, 80), "Connect"))
			{
				NetworkConnectionError error;
				error = Network.Connect (IP, Port);
				if(error == NetworkConnectionError.NoError)
				{
					connected = true;
				}
				else {
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
			}
			else
			{
				
			}
		}
	}
}
