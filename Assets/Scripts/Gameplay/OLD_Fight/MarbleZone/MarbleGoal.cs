using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleGoal : MonoBehaviour {

    private SpriteRenderer _spriteRenderer;

    private MarbleZone _marbleZone;
    public MarbleZone MarbleZone { get { return _marbleZone; } }

    private MarbleGoalBehavior _behavior;

    private float _widthWieght = 1;
    public float WidthWeight { get { return _widthWieght; } }

    public void SetBehavior(MarbleGoalBehavior behavior) {
        _behavior = behavior;
    }

    public void InitGoal(MarbleZone marbleZone, Color color, float widthWieght = 1) {
        _marbleZone = marbleZone;
        _spriteRenderer.color = color;
        _widthWieght = Mathf.Max(widthWieght, 0.001f);
        _marbleZone.AddGoal(this);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Marble") {

            MarbleBall marble = collision.gameObject.GetComponent<MarbleBall>();
            _behavior.Action(marble);
        }
    }

    void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {

    }

    void Update() {
        _behavior.Update();
    }
}
