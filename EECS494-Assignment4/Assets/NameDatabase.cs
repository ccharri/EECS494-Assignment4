using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameDatabase {
	private static Dictionary<string, string> names = new Dictionary<string, string>();

	public static void clearNames() {names.Clear();}
	public static void addName(string key, string name) {names.Add(key, name);}
	public static string getName(string key) {return names[key];}
	public static void removeName(string key) {names.Remove(key);}

	public static Dictionary<string, string>.KeyCollection getKeys() {return names.Keys;}
	public static Dictionary<string, string>.ValueCollection getNames() {return names.Values;}
}
