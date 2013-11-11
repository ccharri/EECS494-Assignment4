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

    //NOTE: A CreepBuff is meaningless without a target!
    private CreepBuff() {}
}
