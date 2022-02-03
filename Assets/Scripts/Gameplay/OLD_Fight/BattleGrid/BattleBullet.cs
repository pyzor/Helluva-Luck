using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBullet : MonoBehaviour {

    private BattleTower _battleTower;
    private Rigidbody2D _rigidbody;

    private float _bulletDelta;
    public int Value { get { return _battleTower.Value; } }
    public Color BulletColor { get { return _battleTower.GetColor(); } }

    public void SetTower(BattleTower battleTower) {
        _battleTower = battleTower;
    }

    public void Init(Vector3 shootDirection, float speed) {
        _bulletDelta = 0;
        gameObject.SetActive(true);

        _rigidbody.velocity = shootDirection * speed;
    }

    public void MoveBackToPool() {
        _battleTower.MoveBackToPool(this);
    }

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start() {

    }

    void Update() {
        _bulletDelta += Time.deltaTime;
        if(_bulletDelta >= 30) {
            MoveBackToPool();
        }
    }
}
