using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSliderHitMarbleObstacleBehavior : MarbleObstacleBehavior {

    private float _hitRateThreshold; // 1 / max hits per sercond
    private float _colorDelta;
    private Color _initialColor;
    private Color _hitColor;


    public TriggerSliderHitMarbleObstacleBehavior(MarbleObstacle obstacle, Color hitColor, float maxHitsPerSecond) : base(obstacle) {
        _hitColor = hitColor;
        _hitRateThreshold = 1.0f - 1.0f / maxHitsPerSecond;
    }

    public override void Init() {
        _initialColor = _obstacle.Color;
    }

    public override void OnCollision(Collision2D collision) {
        if (_colorDelta > _hitRateThreshold)
            return;
        _colorDelta = 1.0f;
        _obstacle.ObstacleLine.MarbleZone.ActionBar.Hit();
    }

    public override void Update() {
        _colorDelta -= Time.deltaTime * 2.0f;
        _colorDelta = Mathf.Max(_colorDelta, 0);
        _obstacle.SetColor(Color.Lerp(_initialColor, _hitColor, _colorDelta));
    }
}
