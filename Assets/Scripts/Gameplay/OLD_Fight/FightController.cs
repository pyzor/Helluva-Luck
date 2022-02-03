using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class FightController : MonoBehaviour {

    [SerializeField] private float Width = 3.7f;
    [SerializeField] private float GridHeight = 5.1f;
    [SerializeField] private float ZoneHeight = 4.1f;

    [SerializeField] private float ActionBarSliderSpeed = 0.5f;


    [SerializeField] private BattleGrid BattleGrid;
    [SerializeField] private MarbleZone MarbleZone;
    [SerializeField] private BulletsManager BulletsManager;

    [SerializeField] private ActionBar ActionBar;
    [SerializeField] private GameObject ActionBarZonePrefab;


    [SerializeField] private GameObject MarbleGoalPrefab;
    [SerializeField] private GameObject MarbleObstacleLinePrefab;

    [SerializeField] private List<GameObject> MarbleObstaclePrefabs;




    public void BeginFight(uint tileHash, uint tileId) {
        BulletsManager.Init();
        BattleGrid.InitGrid(BulletsManager, Width, GridHeight, 2.0f * Mathf.CeilToInt(Random.Range(5, 12))); // Temp Solution
        MarbleZone.InitZone(ActionBar, BulletsManager, Width, ZoneHeight);

        // adding obstacle lines
        Color color = new Color(0.1171875f, 0.1171875f, 0.1171875f) * 2.0f;

        var ol_1 = NewMarbleObstacleLine(0);
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_1, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_1, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_1, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_1, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_1, color, 0));

        var ol_2 = NewMarbleObstacleLine(0.2f);
        new TriggerSliderHitMarbleObstacleBehavior(NewMarbleObstacle(0, ol_2, color, 0), Color.red, 10);
        new TriggerSliderHitMarbleObstacleBehavior(NewMarbleObstacle(0, ol_2, color, 0), Color.red, 10);
        new TriggerSliderHitMarbleObstacleBehavior(NewMarbleObstacle(0, ol_2, color, 0), Color.red, 10);
        new TriggerSliderHitMarbleObstacleBehavior(NewMarbleObstacle(0, ol_2, color, 0), Color.red, 10);

        var ol_3 = NewMarbleObstacleLine(0);
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_3, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_3, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_3, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_3, color, 0));
        new NormalMarbleObstacleBehavior(NewMarbleObstacle(0, ol_3, color, 0));
        
        // todo add marble goals
        new ConvertPlayerPotentialMarbleGoalBehavior(NewMarbleGoal(new Color(0.6f, 0, 0.6f, 0.5f), 0.5f));
        new MultiplyPlayerPotentialMarbleGoalBehavior(NewMarbleGoal(new Color(0, 0.6f, 0, 0.5f)), 2);
        new StackableAddPlayerPotentialMarbleGoalBehavior(NewMarbleGoal(new Color(0.6f, 0.6f, 0, 0.5f)), 1, 0);

        ActionBar.Init(BulletsManager, ActionBarSliderSpeed);

        float oneTenthOfHeight = ActionBar.Height * 0.1f;
        new MultiplyEnemyPotentialActionBarZoneBehavior(NewActionBarZone(new Color(0, 0.6f, 0, 0.5f), oneTenthOfHeight * 1.5f, oneTenthOfHeight), 2);
        new ConvertEnemyPotentialActionBarZoneBehavior(NewActionBarZone(new Color(0.6f, 0, 0.6f, 0.5f), oneTenthOfHeight));
        new MultiplyEnemyPotentialActionBarZoneBehavior(NewActionBarZone(new Color(0, 0.6f, 0, 0.5f), oneTenthOfHeight * 1.5f, oneTenthOfHeight), 2);
    }

    private MarbleGoal NewMarbleGoal(Color color, float weight = 1) {
        var goal = Instantiate(MarbleGoalPrefab, MarbleZone.transform).GetComponent<MarbleGoal>();
        goal.InitGoal(MarbleZone, color, weight);
        return goal;
    }

    private MarbleObstacleLine NewMarbleObstacleLine(float edgePadding = 0) {
        var ol = Instantiate(MarbleObstacleLinePrefab, MarbleZone.transform).GetComponent<MarbleObstacleLine>();
        ol.Init(MarbleZone, edgePadding);
        return ol;
    }

    private MarbleObstacle NewMarbleObstacle(int prefabIndex, MarbleObstacleLine parentLine, Color color, float angularVelocity = 0) {
        var obstacle = Instantiate(MarbleObstaclePrefabs[prefabIndex], parentLine.ObstaclesTransform).GetComponent<MarbleObstacle>();
        obstacle.Init(parentLine, color, angularVelocity);
        parentLine.AddObstacle(obstacle);
        return obstacle;
    }

    private ActionBarZone NewActionBarZone(Color color, float height, float spacing = 0) {
        var zone = Instantiate(ActionBarZonePrefab, ActionBar.HitZonesTransform).GetComponent<ActionBarZone>();
        zone.Init(ActionBar, color, height, spacing);
        return zone;
    }

}

