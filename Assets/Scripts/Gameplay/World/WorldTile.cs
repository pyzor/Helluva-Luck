using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WorldTile : MonoBehaviour {

    private int _ID;

    private int _pathDeviation;
    private Vector2 _tileCoords;
    private Vector2 _pathDirection;

    private bool _removing = false;
    private float _moveDelta;

    private WorldTileBehavior _behavior;

    private WorldTile _nextTile;
    private WorldTile _previousTile;

    private MeshRenderer _meshRenderer;

    /// <summary>
    /// Helps the path not to cross itself very often.
    /// Indicates where the next tile will be relative to the previous one.
    /// </summary>
    public int PathDeviation { get { return _pathDeviation; } }
    public Vector2 TileCoords { get { return _tileCoords; } }

    /// <summary>
    /// Direction to the next tile.
    /// </summary>
    public Vector2 PathDirection { get { return _pathDirection; } }

    public bool IsRemoving { get { return _removing; } }
    public int ID { get { return _ID; } }

    public void Init(WorldTile previousTile, float noiseScale = 1) {
        _previousTile = previousTile;
        _moveDelta = 0;

        if (_previousTile != null) {
            _ID = _previousTile.ID + 1;
            _previousTile.SetNextTile(this);
            _pathDeviation = _previousTile.PathDeviation;
            _tileCoords = _previousTile.TileCoords + _previousTile.PathDirection;
            _pathDirection = _previousTile.PathDirection;
            UpdatePathDirection();
        } else {    // if this is the first tile
            _ID = 0;
            _tileCoords = new Vector2(0, 0);
            _pathDeviation = GetNewDeviation(); // GetNewDeviation() requires _tileNoiseValue to get correct _pathDeviation
            _pathDirection = (_pathDeviation != 1) ? new Vector2(0, 1) : new Vector2(1, 0);
        }

        transform.localPosition = new Vector3(_tileCoords.x, -2f, _tileCoords.y);
    }

    public void SetBehavior(WorldTileBehavior behavior) {
        _behavior = behavior;
        _behavior.SetWorldTile(this);
        _behavior.UpdateMaterial(_meshRenderer);

        gameObject.SetActive(true);
        _behavior.OnEnable();
    }

    public void ChangeDeviation() {
        var possibleDeviation = _pathDeviation + GetNewDeviation();
        if (possibleDeviation == 0) {
            _pathDeviation = (_pathDeviation < possibleDeviation) ? 1 : -1;
        } else {
            _pathDeviation = (possibleDeviation > 2) ? 1 : (possibleDeviation < -2) ? -1 : possibleDeviation;
        }
        UpdatePathDirection();
    }

    public void SetNextTile(WorldTile nextTile) {
        _nextTile = nextTile;
    }

    public void RemovePreviousTile() {
        _previousTile = null;
    }

    public void BehaviorAction() {
        _behavior.OnAction();
    }

    public void BehaviorHopOver() {
        _behavior.OnHopOver();
    }

    public void InitiateRemoving() {
        if (_removing)
            return;

        if (_nextTile != null) {
            _nextTile.RemovePreviousTile();
            _nextTile = null;
        }
        _removing = true;
        _ID = -1;

        _behavior.OnDisable();
    }

    public void RemoveInstantly() {
        if (_nextTile != null) {
            _nextTile.RemovePreviousTile();
            _nextTile = null;
        }
        _removing = true;
        _ID = -1;

        _behavior.OnDestroy();

        WorldGenerationMaster.Instance.MoveBackToPool(this);
    }

    void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        gameObject.SetActive(false);
    }

    void Update() {
        _behavior.Update();

        if (_removing) {
            _moveDelta -= Time.deltaTime;
            _moveDelta = Mathf.Min(_moveDelta, 1);

            transform.localPosition = Vector3.Lerp(new Vector3(_tileCoords.x, -2f, _tileCoords.y), transform.localPosition, _moveDelta);

            if (_moveDelta <= 0) {
                _removing = false;
                _moveDelta = 0;

                _behavior.OnDestroy();

                WorldGenerationMaster.Instance.MoveBackToPool(this);
            }
        } else {
            if (_moveDelta != 1) {
                _moveDelta += Time.deltaTime;
                _moveDelta = Mathf.Min(_moveDelta, 1);

                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(_tileCoords.x, 0, _tileCoords.y), _moveDelta);
            }
        }
    }

    private int GetNewDeviation() {
        return Mathf.RoundToInt(Random.Range(0.0f, 1.0f)) * 2 - 1;
    }

    private void UpdatePathDirection() {
        _pathDirection = CalculatePathDirection();
    }

    private Vector2 CalculatePathDirection() {
        if(_previousTile == null)
            return (_pathDeviation > 0) ? Vector2.right : Vector2.up;

        if (_pathDeviation > _previousTile.PathDeviation) {
            return new Vector2(_pathDirection.y, -_pathDirection.x); // rotate clockwise
        } else if (_pathDeviation < _previousTile.PathDeviation) {
            return new Vector2(-_pathDirection.y, _pathDirection.x); // rotate counterclockwise
        } else {
            return _pathDirection;
        }
    }

    private void OnDestroy() {
        if (_nextTile != null) {
            _nextTile.RemovePreviousTile();
            _nextTile = null;
        }
        _removing = true;
        _ID = -1;

        _behavior.OnDestroy();
    }
}
