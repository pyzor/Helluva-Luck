

class ConvertPlayerPotentialMarbleGoalBehavior : MarbleGoalBehavior {

    private bool _free;

    public ConvertPlayerPotentialMarbleGoalBehavior(MarbleGoal marbleGoal, bool free = false) : base(marbleGoal) {
        _free = free;
    }

    public override void Action(MarbleBall marble) {
        _marbleGoal.MarbleZone.BulletsManager.ConvertPlayerPotential(_free);
        marble.MoveBackToPool();
    }

    public override void Init() {
        // todo - change text to ( "C" )
    }

    public override void Update() {

    }
}