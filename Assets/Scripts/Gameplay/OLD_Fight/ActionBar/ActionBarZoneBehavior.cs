using UnityEngine;

public abstract class ActionBarZoneBehavior {

    protected ActionBarZone _actionBarZone;

    protected ActionBarZoneBehavior(ActionBarZone actionBarZone) {
        _actionBarZone = actionBarZone;
        _actionBarZone.SetBehavior(this);
        Init();
    }
    public abstract void Action();
    public abstract void Init();
    public abstract void Update();


}
