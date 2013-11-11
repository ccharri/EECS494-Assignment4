using UnityEngine;
using System.Collections;

public abstract class CreepBuff : Buff 
{
    protected Creep target;

    public CreepBuff(Creep target_) : base()
    {
        target = target_;
        onApplication();
    }
    public CreepBuff(Creep target_, Unit owner_) : base(owner_)
    {
        target = target_;
        onApplication();
    }

    public override bool FixedUpdate()
    //DOES: Returns if the buff has expired or not
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            onRemoval();
            return true;
        }
        return false;
    }

    //NOTE: A CreepBuff is meaningless without a target!
    private CreepBuff() {}
}
