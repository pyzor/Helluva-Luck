using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWorldMovement : MonoBehaviour {

    [SerializeField] private float Speed = 3;
    [SerializeField] private float JumpHeight = 1;
    [SerializeField] private float DiceJumpSpeed = 2; 
    [SerializeField] private float DiceJumpHeight = 1;


    //[SerializeField] private LavaOffset lavaOffset;

    private int _playerNextPosition, _playerCurrentPosition;

    private WorldTile _currentWorldTile;
    private WorldTile _nextWorldTile;

    private float _stepDelta = 0;


    void Start() {
        var tile = WorldGenerationMaster.Instance.GetFirstTile();
        _playerCurrentPosition = (tile != null) ? tile.ID : 0;
        _playerNextPosition = _playerCurrentPosition;
    }


    public void DiceJump(float dt) {
        transform.localPosition = new Vector3(0, 0.3f + DiceJumpHeight * Mathf.Sin(Mathf.PI * dt), 0);
    }


    public void MovePlayer(int amount) {
        if (amount <= 0)
            return;
        _playerNextPosition = _playerCurrentPosition + amount;

        // TODO find a better way to move?
        _currentWorldTile = WorldGenerationMaster.Instance.GetTile(_playerCurrentPosition);
        _nextWorldTile = WorldGenerationMaster.Instance.GetTile(_playerCurrentPosition+1);
    }
    public void AddSteps(int amount) {
        _playerNextPosition += amount;
    }

    public float GetDiceJumpSpeed() {
        return DiceJumpSpeed;
    }

    public bool IsMoving() {
        return _playerCurrentPosition != _playerNextPosition;
    }

    void Update() {
        if (_playerCurrentPosition == _playerNextPosition) {
            return;
        };

        if (_stepDelta < 1) { // 1 tile = 1 step
            _stepDelta += Time.deltaTime * Speed;
            _stepDelta = Mathf.Min(_stepDelta, 1);

            var curTilePos = _currentWorldTile.transform.localPosition + Vector3.zero;
            curTilePos.y = 0;
            var nextTilePos = _nextWorldTile.transform.localPosition + Vector3.zero;
            nextTilePos.y = 0;
            WorldGenerationMaster.Instance.transform.localPosition = -Vector3.Lerp(curTilePos, nextTilePos, _stepDelta);
            //lavaOffset.UpdateLavaOffset(WorldGenerationMaster.Instance.transform.localPosition.x, WorldGenerationMaster.Instance.transform.localPosition.z);

            transform.localPosition = new Vector3(0, 0.3f + JumpHeight * Mathf.Sin(Mathf.PI * _stepDelta), 0);

            var rotateAround = new Vector3(_currentWorldTile.PathDirection.y, 0, -_currentWorldTile.PathDirection.x);
            transform.Rotate(rotateAround, Time.deltaTime * Speed * 360.0f); ;
        }

        if (_stepDelta >= 1) { 
            _playerCurrentPosition += 1;
            transform.localRotation = Quaternion.identity;
            _stepDelta = 0;
            _currentWorldTile = _nextWorldTile;
            if (_playerCurrentPosition == _playerNextPosition) {
                _currentWorldTile.BehaviorAction();
                WorldGenerationMaster.Instance.MakeFirstTile(_playerCurrentPosition);
                return;
            } else {    
                _currentWorldTile.BehaviorHopOver();
            }
            _nextWorldTile = WorldGenerationMaster.Instance.GetTile(_playerCurrentPosition + 1);
            WorldGenerationMaster.Instance.MakeFirstTile(_playerCurrentPosition);
        }
    }
}
