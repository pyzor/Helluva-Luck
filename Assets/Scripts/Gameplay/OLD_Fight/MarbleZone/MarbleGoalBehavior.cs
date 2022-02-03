

public abstract class MarbleGoalBehavior {

    protected MarbleGoal _marbleGoal;

    protected MarbleGoalBehavior(MarbleGoal marbleGoal) {
        _marbleGoal = marbleGoal;
        _marbleGoal.SetBehavior(this);
        Init();
    }

    public abstract void Action(MarbleBall marble);
    public abstract void Init();
    public abstract void Update();

}

