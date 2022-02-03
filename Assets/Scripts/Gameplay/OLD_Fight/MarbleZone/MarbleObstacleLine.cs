using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleObstacleLine : MonoBehaviour {

    [SerializeField] private Transform _transform;

    private MarbleZone _marbleZone;
    private float _edgePadding = 0;

    private List<MarbleObstacle> _obstacles;

    public MarbleZone MarbleZone { get { return _marbleZone; } }
    public Transform ObstaclesTransform { get { return _transform; } }

    public void Init(MarbleZone marbleZone, float edgePadding) {
        _marbleZone = marbleZone;
        _edgePadding = edgePadding;

        _marbleZone.AddObstacleLine(this);
    }

    public void SetEdgePadding(float edgePadding) {
        _edgePadding = edgePadding;
    }

    public void UpdateLayout() {
        if (_obstacles.Count > 1) {
            for (int i = 0; i < _obstacles.Count; i++) {
                _obstacles[i].transform.localScale = new Vector3(0.1f, 0.1f, 1);
                //_obstacles[i].transform.localPosition = new Vector3(i * (1 - _edgePadding) / _obstacles.Count + _edgePadding * 0.5f - 0.5f, 0);
                _obstacles[i].transform.localPosition = new Vector3((_edgePadding * 0.5f) + i * ((1 - _edgePadding) / Mathf.Max(_obstacles.Count - 1, 0.5f)) - 0.5f, 0);
            }
        } else if(_obstacles.Count > 0) {
            _obstacles[0].transform.localScale = new Vector3(0.1f, 0.1f, 1);
            _obstacles[0].transform.localPosition = new Vector3(0, 0);
        }
    }

    public void AddObstacle(MarbleObstacle obstacle) {
        _obstacles.Add(obstacle);
        UpdateLayout();
    }

    void Awake() {
        _obstacles = new List<MarbleObstacle>();
    }

    void Start() {

    }

    void Update() {

    }

    private void OnDestroy() {
        if(_obstacles != null) {
            for(int i = 0; i < _obstacles.Count; i++) {
                Destroy(_obstacles[i].gameObject);
            }
            _obstacles.Clear();
        }
    }
}
