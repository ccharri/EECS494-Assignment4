using UnityEngine;
using System.Collections;

public class ArcaneTower : Tower 
{
    public GameObject magicMissile;

    public void Start()
    {
        base.Init(1000, 10000);
    }

    public void Init(string pid_)
    {
        base.Init("Arcane Tower", pid_);
    }

    public override void fire()
    {
        base.fire();
        GameObject proj = Instantiate(magicMissile, transform.position, transform.rotation) as GameObject;
        MagicMissile p = proj.GetComponent<MagicMissile>();
        p.Init(target);
    }

    public override string getDescription()
    {
        return "Arcane Tower\nArguably one of the greatest achievements of the human race,\n" +
            " the Arcane Tower is little more than a bundle of sticks, an artificial mana\n" +
            " crystal, and a targeting glyph. They made them by the thousands, and secured" +
            " their vast borders against demonic attack.";
    }
}
