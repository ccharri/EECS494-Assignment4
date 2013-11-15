using UnityEngine;
using System.Collections;

public class ArcaneTower : Tower 
{
    public void Init()
    {
        base.Init(1000, 1);
    }

    public override void fire()
    {
        base.fire();
        GameObject proj = Instantiate(Resources.Load<Object>("Projectiles/MagicMissile"), transform.position, transform.rotation) as GameObject;
        proj.SendMessage("MagicMissile", target);
    }

    public override string getDescription()
    {
        return "Arcane Tower\nArguably one of the greatest achievements of the human race,\n" +
            " the Arcane Tower is little more than a bundle of sticks, an artificial mana\n" +
            " crystal, and a targeting glyph. They made them by the thousands, and secured" +
            " their vast borders against demonic attack.";
    }
}
