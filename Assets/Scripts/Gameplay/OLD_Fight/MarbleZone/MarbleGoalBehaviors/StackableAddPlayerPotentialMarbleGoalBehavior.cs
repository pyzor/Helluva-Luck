

class StackableAddPlayerPotentialMarbleGoalBehavior : MarbleGoalBehavior {

    private uint _value;
    private uint _step;

    public StackableAddPlayerPotentialMarbleGoalBehavior(MarbleGoal marbleGoal, uint initialValue, uint step) : base(marbleGoal) {
        _value = initialValue;
        _step = step;
    }

    public override void Action(MarbleBall marble) {
        _marbleGoal.MarbleZone.BulletsManager.AddToPlayerPotential(_value);
        _value += _step;
        // todo - change text to ( "+"+_value )
        marble.MoveBackToPool();
    }

    public override void Init() {
        // todo - change text to ( "+"+_value )
    }

    public override void Update() {

    }
}
