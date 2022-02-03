using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTower : MonoBehaviour {

    [SerializeField] private GameObject BattleBulletPrefab;

    [SerializeField] private SpriteRenderer TowerBodySR;
    [SerializeField] private SpriteRenderer TowerMuzzleSR;

    [SerializeField] private AnimationCurve TowerRotationCurve;

    [SerializeField] private float MaxAngle = 85;
    [SerializeField] private float RotationSpeed = 0.5f;


    [SerializeField] private float ShootDelay = 0.1f; // Less delay = more fire rate; To avoid dividing each frame (for ex. 1 / 10 = 0.1 delay );


    //private const float _twoPI = 2 * Mathf.PI;

    private float _angleDelta = 0;

    private Color _color = Color.white;
    private int _value;
    private BattleGrid _battleGrid;
    private float _shootDelta;

    private Queue<BattleBullet> _bulletPool;
    private List<BattleBullet> _activeBullets;

    public int ActiveBulletsCount { get { return _activeBullets.Count; } }
    public int Value { get { return _value; } }

    public void SetColor(Color color, int value) {
        _color = color;

        var towerColor = _color * 0.6f;
        towerColor.a = 1;
        TowerBodySR.color = towerColor;
        TowerMuzzleSR.color = towerColor;

        _value = value;
    }

    public Color GetColor() {
        return _color;
    }

    public void SetBattleGrid(BattleGrid battleGrid) {
        _battleGrid = battleGrid;
    }

    void Awake() {
        _bulletPool = new Queue<BattleBullet>();
        _activeBullets = new List<BattleBullet>();

    }


    void Update() {
        AngleUpdate();


        _shootDelta += Time.deltaTime;
        _shootDelta = Mathf.Min(_shootDelta, ShootDelay);
        if(_shootDelta >= ShootDelay) {
            Shoot();
        }
    }

    // Override this delegate inside BattleGrid class to avoid each frame checking which tower is asking for available bullets
    public Func<bool> ConsumeAvailableBullet { get; set; }

    public void Shoot() {
        if (!ConsumeAvailableBullet()) {
            return;
        }

        BattleBullet bullet;

        if (_bulletPool.Count == 0) {
            bullet = Instantiate(BattleBulletPrefab, transform).GetComponent<BattleBullet>();
            bullet.transform.localScale = TowerMuzzleSR.transform.localScale;
            bullet.SetTower(this);
        } else {
            bullet = _bulletPool.Dequeue();
        }
        bullet.transform.position = TowerMuzzleSR.transform.position;
        bullet.Init((TowerMuzzleSR.transform.position - TowerBodySR.transform.position).normalized, 2);

        var bulletSpriteRenderer = bullet.GetComponent<SpriteRenderer>();
        bulletSpriteRenderer.color = _color;

        _activeBullets.Add(bullet);
        _shootDelta = 0;
    }

    private void AngleUpdate() {
        _angleDelta += Time.deltaTime * RotationSpeed;
        if(_angleDelta > 1) {
            _angleDelta -= 1;
        }
        TowerBodySR.transform.localRotation = Quaternion.Euler(0, 0, (TowerRotationCurve.Evaluate(_angleDelta)) * MaxAngle);
    }

    public void MoveBackToPool(BattleBullet bullet) {
        if (!_activeBullets.Remove(bullet))
            return;

        bullet.gameObject.SetActive(false);
        _bulletPool.Enqueue(bullet);
    }

    private void OnDestroy() {
        for (int i = 0; i < _activeBullets.Count; i++) {
            if(_activeBullets[i] != null)
                Destroy(_activeBullets[i].gameObject);
        }
        for (int i = 0; i < _bulletPool.Count; i++) {
            var bullet = _bulletPool.Dequeue();
            if (bullet != null)
                Destroy(bullet.gameObject);
        }
    }
}
