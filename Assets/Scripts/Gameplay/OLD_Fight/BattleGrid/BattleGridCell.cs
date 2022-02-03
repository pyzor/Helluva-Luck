using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGridCell : MonoBehaviour {

    private int _indexX, _indexY;
    private BattleGrid _battleGrid;
    private int Value { get { return _battleGrid.GetCellValue(_indexX, _indexY); } }

    public void Init(int x, int y, BattleGrid battleGrid) {
        _indexX = x;
        _indexY = y;
        _battleGrid = battleGrid;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "BattleArenaBullet") {
            BattleBullet bullet = collision.gameObject.GetComponent<BattleBullet>();
            if (bullet == null)
                return;
            if (bullet.Value != Value) {
                _battleGrid.CaptureCell(_indexX, _indexY, bullet.Value, bullet.BulletColor);
                bullet.MoveBackToPool();
            }
        }
    }

    void Start() {

    }

    void Update() {

    }
}
