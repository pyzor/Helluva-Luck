using UnityEngine;


public abstract class MarbleObstacleBehavior {


    protected MarbleObstacle _obstacle;

    protected MarbleObstacleBehavior(MarbleObstacle obstacle) {
        _obstacle = obstacle;
        _obstacle.SetBehavior(this);
        Init();
    }

    public abstract void OnCollision(Collision2D collision);

    public abstract void Init();

    public abstract void Update();



}
