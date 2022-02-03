using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleBall : MonoBehaviour {

    private MarbleDropper _dropper;
    private Rigidbody2D _rigidbody;

    private float _time;

    public void SetDropper(MarbleDropper dropper) {
        _dropper = dropper;
    }

    public void Init(Vector3 shootDirection, float speed) {
        gameObject.SetActive(true);

        _rigidbody.velocity = shootDirection * speed;
    }

    public void AddForce(Vector2 force) {
        _rigidbody.AddForce(force, ForceMode2D.Force);
    }

    public void MoveBackToPool() {
        _time = 0;
        _rigidbody.velocity = Vector2.zero;
        _dropper.MoveBackToPool(this);
    }

    void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start() {

    }

    void Update() {
        _time += Time.deltaTime;
        if (_time >= 20) {
            MoveBackToPool();
        }

        if (_rigidbody.IsSleeping()) {
            _dropper.MoveBackToPool(this);
        }
    }
}