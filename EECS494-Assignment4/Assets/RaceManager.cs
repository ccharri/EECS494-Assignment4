using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour {
	//Dictionary Workaround
	//
	public List<string> raceMapKeys = new List<string>();
	public List<Race> raceMapValues = new List<Race>();
	//

	public Dictionary<string, Race> raceMap = new Dictionary<string, Race>();
	void Awake()
	{
//		//Zip up dictionary
//		//
//		for(int i = 0; i < raceMapKeys.Count; i++)
//		{
//			raceMap.Add (raceMapKeys[0], raceMapValues[0]);
//		}
		//
	}

	// Use this for initialization
	void Start () {
		//Zip up dictionary
		//
		for(int i = 0; i < raceMapKeys.Count; i++)
		{
			Debug.Log ("Adding {"+raceMapKeys[i]+":"+raceMapValues[i]+"}");
			raceMapValues[i].Zip();
			raceMap.Add (raceMapKeys[0], raceMapValues[0]);
		}

		Debug.Log("RaceManager created dictionary = " + raceMap);
		foreach(string key in raceMap.Keys)
		{
			Debug.Log ("Race = " + key + ": " + raceMap[key]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
