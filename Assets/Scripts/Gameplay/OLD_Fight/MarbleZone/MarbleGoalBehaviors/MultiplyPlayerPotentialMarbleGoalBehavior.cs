

public class MultiplyPlayerPotentialMarbleGoalBehavior : MarbleGoalBehavior {

    private float _multiplyer;

    public MultiplyPlayerPotentialMarbleGoalBehavior(MarbleGoal marbleGoal, float multiplyer) : base(marbleGoal) {
        _multiplyer = multiplyer;
    }

    public override void Action(MarbleBall marble) {
        _marbleGoal.MarbleZone.BulletsManager.MultiplyPlayerPotential(_multiplyer);
        marble.MoveBackToPool();
    }

    public override void Init() {
        // todo - change text to ( "x"+_multiplyer )
    }

    public override void Update() {

    }
}
