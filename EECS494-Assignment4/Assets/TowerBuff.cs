using UnityEngine;
using System.Collections;

public class TowerBuff : Buff {

	protected Tower target;

    public TowerBuff(Tower target_) : base()
    {
        target = target_;
        onApplication();
    }
    public TowerBuff(Tower target_, Unit owner_) : base(owner_)
    {
        target = target_;
        onApplication();
    }

    //NOTE: A TowerBuff is meaningless without a target!
    private TowerBuff() {}
}
