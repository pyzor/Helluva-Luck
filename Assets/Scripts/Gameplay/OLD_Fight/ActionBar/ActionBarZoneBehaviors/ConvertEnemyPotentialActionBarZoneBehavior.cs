

public class ConvertEnemyPotentialActionBarZoneBehavior : ActionBarZoneBehavior {

    private bool _free;

    public ConvertEnemyPotentialActionBarZoneBehavior(ActionBarZone actionBarZone, bool free = false) : base(actionBarZone) {
        _free = free;
    }

    public override void Action() {
        _actionBarZone.ActionBar.BulletsManager.ConvertEnemyPotential(_free);
    }

    public override void Init() {

    }

    public override void Update() {

    }
}
