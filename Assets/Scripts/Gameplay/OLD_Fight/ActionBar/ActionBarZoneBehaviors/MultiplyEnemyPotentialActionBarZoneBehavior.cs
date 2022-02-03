

class MultiplyEnemyPotentialActionBarZoneBehavior : ActionBarZoneBehavior {

    private float _multiplyer;

    public MultiplyEnemyPotentialActionBarZoneBehavior(ActionBarZone actionBarZone, float multiplyer) : base(actionBarZone) {
        _multiplyer = multiplyer;
    }

    public override void Action() {
        _actionBarZone.ActionBar.BulletsManager.MultiplyEnemyPotential(_multiplyer);
    }

    public override void Init() {

    }

    public override void Update() {

    }
}
