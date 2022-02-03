using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleDropper : MonoBehaviour {

    [SerializeField] private GameObject MarbleBallPrefab;
    [SerializeField] private AnimationCurve DropperMuzzleRotationCurve;

    private SpriteRenderer _bodySpriteRenderer;
    private SpriteRenderer _muzzleBodySpriteRenderer;

    [SerializeField] private Transform _muzzleRoot;
    [SerializeField] private Transform _muzzleBody;
    [SerializeField] private Transform _muzzleEnd;

    [SerializeField] private float MaxMuzzleAngle = 33;
    [SerializeField] private float RotationSpeed = 0.2f;


    private Queue<MarbleBall> _marblePool;
    private List<MarbleBall> _activeMarbles;

    private Color _color;
    private MarbleZone _marbleZone;

    private float _shootDelta;
    private float _angleDelta;

    public void SetColor(Color color) {
        _bodySpriteRenderer.color = color;
        _muzzleBodySpriteRenderer.color = color;
        _color = color;
    }

    public void SetMarbleZone(MarbleZone marbleZone) {
        _marbleZone = marbleZone;
    }

    /*public Vector3 GetSpawnOffset() {
        return transform.localPosition + 
            new Vector3(
                _bodySpriteRenderer.transform.localPosition.x * _bodySpriteRenderer.transform.localScale.x * transform.localScale.x, 
                _bodySpriteRenderer.transform.localPosition.y * _bodySpriteRenderer.transform.localScale.y * transform.localScale.y, 0);
    }*/

    public Vector3 GetAimVector() {
        return (_muzzleEnd.transform.position - _muzzleRoot.transform.position).normalized;
    }

    public void MoveBackToPool(MarbleBall marble) {
        if (!_activeMarbles.Remove(marble))
            return;

        marble.gameObject.SetActive(false);
        _marblePool.Enqueue(marble);
    }

    public void Shoot() {
        MarbleBall marble;

        if (_marblePool.Count == 0) {
            marble = Instantiate(MarbleBallPrefab, _marbleZone.transform).GetComponent<MarbleBall>();
            marble.transform.localScale = new Vector3(
            _muzzleBody.transform.localScale.x * transform.localScale.x * 0.8f,
            _muzzleBody.transform.localScale.y * transform.localScale.y * 0.8f, 1);
            marble.SetDropper(this);
        } else {
            marble = _marblePool.Dequeue();
        }
        marble.transform.position = _muzzleEnd.transform.position;
        marble.Init(GetAimVector(), 3.5f);
        //marble.AddForce(_marbleZone.GetDropperForceDirection() * _marbleZone.GetDropperSpeed() * 10);
        //marble.AddVelocity(_marbleZone.GetDropperForceDirection(), _marbleZone.GetDropperSpeed());

        var marbleSpriteRenderer = marble.GetComponentInChildren<SpriteRenderer>();
        marbleSpriteRenderer.color = _color;

        _activeMarbles.Add(marble);

        _shootDelta = 0;
    }

    void Awake() {
        _bodySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _muzzleBodySpriteRenderer = _muzzleBody.gameObject.GetComponent<SpriteRenderer>();

        _marblePool = new Queue<MarbleBall>();
        _activeMarbles = new List<MarbleBall>();
    }

    void Start() {

    }

    void Update() {
        AngleUpdate();

        _shootDelta += Time.deltaTime;
        if (_shootDelta >= 1.0f) {
            _shootDelta -= 1;
            Shoot();
        }
    }

    private void AngleUpdate() {
        _angleDelta += Time.deltaTime * RotationSpeed;
        if (_angleDelta > 1) {
            _angleDelta -= 1;
        }
        _muzzleRoot.transform.localRotation = Quaternion.Euler(0, 0, (DropperMuzzleRotationCurve.Evaluate(_angleDelta)) * MaxMuzzleAngle);
    }

    private void OnDestroy() {
        for (int i = 0; i < _activeMarbles.Count; i++) {
            if (_activeMarbles[i] != null)
                Destroy(_activeMarbles[i].gameObject);
        }
        for (int i = 0; i < _marblePool.Count; i++) {
            var marble = _marblePool.Dequeue();
            if (marble != null)
                Destroy(marble.gameObject);
        }
    }
}
