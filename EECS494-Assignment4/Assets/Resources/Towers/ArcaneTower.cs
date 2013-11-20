﻿using UnityEngine;
using System.Collections;

public class ArcaneTower : Tower 
{
    public GameObject magicMissile;

	void Awake() 
	{
		id = "arcaneTower";
		name = "Arcane Tower";
	}

	public void LoadPrefabs()
	{
		id = "arcaneTower";
		name = "Arcane Tower";
		prefab = Resources.Load ("Towers/ArcaneTower") as GameObject;
		if(prefab != null)
			Debug.Log ("Loaded ArcaneTower prefab " + prefab);
		else
			Debug.Log ("ArcaneTower prefab = null");
		magicMissile = Resources.Load ("Projectiles/MagicMissile") as GameObject;
		if(magicMissile != null)
			Debug.Log ("Loaded MagicMissile prefab " + magicMissile);
		else
			Debug.Log ("MagicMissile prefab = null");
	}

    public override void Init(string pid_)
    {
        base.Init("Arcane Tower", pid_, 1, 1);
    }

    protected override void fire()
    {
        base.fire();
        GameObject proj = Instantiate(magicMissile, transform.position, transform.rotation) as GameObject;
        MagicMissile p = proj.GetComponent<MagicMissile>();
        p.Init(target, this);
    }

    public override string getDescription()
    {
        return "Arcane Tower\nArguably one of the greatest achievements of the human race,\n" +
            " the Arcane Tower is little more than a bundle of sticks, an artificial mana\n" +
            " crystal, and a targeting glyph. They made them by the thousands, and secured" +
            " their vast borders against demonic attack.";
    }
}
