using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleZone : MonoBehaviour {



    [SerializeField] private GameObject MarbleGoalSpacerPrefab;

    [SerializeField] private GameObject MarbleDropperPrefab;
    [SerializeField] private AnimationCurve MarbleDropperMovementCurve;

    [SerializeField] private Sprite BackgroundSprite;
    [SerializeField] private Material BackgroundMaterial;

    [SerializeField] private float DropperMovementSpeed = 0.1f;

    private EdgeCollider2D _edgeCollider;
    private Vector3 _offset;
    private Vector2 _bounds;
    private float _obstacleLinesEdgePadding = 0.5f;

    private GameObject _background;
    private MarbleDropper _marbleDropper;
    private ActionBar _actionBar;
    private BulletsManager _bulletsManager;

    private float _dropperDelta;

    private List<MarbleGoal> _marbleGoals;
    private List<MarbleGoalSpacer> _marbleGoalSpacers;
    private List<MarbleObstacleLine> _marbleObstacleLines;

    private bool _initDone = false;

    public ActionBar ActionBar { get { return _actionBar; } }
    public BulletsManager BulletsManager { get { return _bulletsManager; } }
    public Vector3 Offset { get { return _offset; } }

    public void InitZone(ActionBar actionBar, BulletsManager bulletsManager, float width, float height) {
        _actionBar = actionBar;
        _bulletsManager = bulletsManager;
        _offset = new Vector3(width * 0.5f, height * 0.5f);
        _bounds = new Vector2(width, height);

        Vector2 p = new Vector2(_offset.x, _offset.y);
        _edgeCollider.SetPoints(new List<Vector2>() {
            new Vector2(-p.x, p.y),
            new Vector2(p.x, p.y),
            new Vector2(p.x, -p.y),
            new Vector2(-p.x, -p.y),
            new Vector2(-p.x, p.y),
        });

        if (_background != null) {
            Destroy(_background);
        }

        _background = new GameObject("Background", typeof(SpriteRenderer));
        Transform gbTransform = _background.transform;
        gbTransform.SetParent(gameObject.transform, false);
        gbTransform.localPosition = Vector3.zero;
        gbTransform.localScale = new Vector3(width, height, 1);
        SpriteRenderer bgSpriteRenderer = _background.GetComponent<SpriteRenderer>();
        bgSpriteRenderer.material = BackgroundMaterial;
        bgSpriteRenderer.sprite = BackgroundSprite;
        bgSpriteRenderer.sortingOrder = -70;
        bgSpriteRenderer.color = new Color(0.1171875f, 0.1171875f, 0.1171875f);

        DestroyMarbleDropper();

        _marbleDropper = Instantiate(MarbleDropperPrefab, transform).GetComponent<MarbleDropper>();
        _marbleDropper.transform.localPosition = new Vector3(0, _offset.y);
        _marbleDropper.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        _marbleDropper.SetColor(new Color(0, 0.6f, 0));
        _marbleDropper.SetMarbleZone(this);

        _dropperDelta = 0;

        // todo clear _marbleGoals and _marbleObstacles

        DestroyMarbleGoals();
        DestroyMarbleObstacleLines();

        _initDone = true;
    }

    void Awake() {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _marbleGoals = new List<MarbleGoal>();
        _marbleGoalSpacers = new List<MarbleGoalSpacer>();
        _marbleObstacleLines = new List<MarbleObstacleLine>();
    }

    void Start() {

    }

    void Update() {
        if (!_initDone)
            return;

        DropperPositionUpdate();
    }

    private void DestroyMarbleGoals() {
        if (_marbleGoals != null) {
            for (int i = 0; i < _marbleGoals.Count; i++) {
                Destroy(_marbleGoals[i].gameObject);
            }
            _marbleGoals.Clear();
        }
    }

    private void DestroyMarbleGoalSpacers() {
        if (_marbleGoalSpacers != null) {
            for (int i = 0; i < _marbleGoalSpacers.Count; i++) {
                Destroy(_marbleGoalSpacers[i].gameObject);
            }
            _marbleGoalSpacers.Clear();
        }
    }

    private void DestroyMarbleObstacleLines() {
        if (_marbleObstacleLines != null) {
            for (int i = 0; i < _marbleObstacleLines.Count; i++) {
                Destroy(_marbleObstacleLines[i].gameObject);
            }
            _marbleObstacleLines.Clear();
        }
    }

    private void DestroyMarbleDropper() {
        if (_marbleDropper != null) {
            Destroy(_marbleDropper.gameObject);
        }
    }

    void OnDestroy() {
        DestroyMarbleGoals();
        DestroyMarbleObstacleLines();
        DestroyMarbleGoalSpacers();
    }


    private void FormatGoals() {
        DestroyMarbleGoalSpacers();

        float widthUnit = 0;
        for (int i = 0; i < _marbleGoals.Count; i++) {
            widthUnit += _marbleGoals[i].WidthWeight;
        }
        widthUnit = _bounds.x / widthUnit;

        float xOffset = 0;
        for (int i = 0; i < _marbleGoals.Count; i++) {
            var marbleGoal = _marbleGoals[i];
            marbleGoal.transform.localScale = new Vector3(widthUnit * marbleGoal.WidthWeight, _bounds.y * 0.05f, 1);
            marbleGoal.transform.localPosition = new Vector3(xOffset + marbleGoal.transform.localScale.x * 0.5f - _offset.x, marbleGoal.transform.localScale.y * 0.5f - _offset.y);
            if (i > 0) {
                var spacer = Instantiate(MarbleGoalSpacerPrefab, transform).GetComponent<MarbleGoalSpacer>();
                spacer.SetColor(new Color(0.1171875f, 0.1171875f, 0.1171875f) * 2.0f);

                spacer.transform.localScale = new Vector3(0.2f, _bounds.y * 0.05f, 1);

                spacer.SetSpikeScale(spacer.transform.localScale.x / spacer.transform.localScale.y);
                spacer.transform.localPosition = new Vector3(xOffset - _offset.x, spacer.transform.localScale.y * 0.5f - _offset.y, 1);

                _marbleGoalSpacers.Add(spacer);
            }
            xOffset += marbleGoal.transform.localScale.x;
        }
    }

    public void AddGoal(MarbleGoal marbleGoal) {
        _marbleGoals.Add(marbleGoal);
        FormatGoals();
    }

    private void FormatObstacleLines() {
        if(_marbleObstacleLines.Count > 1) {
            for (int i = 0; i < _marbleObstacleLines.Count; i++) {
                _marbleObstacleLines[i].transform.localScale = new Vector3(_bounds.x, _bounds.x, 1);
                //_marbleObstacleLines[i].transform.localPosition = new Vector3(0, -_bounds.y * (i * (1 - _obstacleLinesEdgePadding) / _marbleObstacleLines.Count + _obstacleLinesEdgePadding * 0.5f) + _offset.y);
                _marbleObstacleLines[i].transform.localPosition = new Vector3(0,
                    -_bounds.y * ((_obstacleLinesEdgePadding * 0.5f) + i * ((1 - _obstacleLinesEdgePadding) / Mathf.Max(_marbleObstacleLines.Count - 1, 0.5f)) - 0.5f));

            }
        }else if(_marbleObstacleLines.Count > 0) {
            _marbleObstacleLines[0].transform.localScale = new Vector3(_bounds.x, _bounds.x, 1);
            _marbleObstacleLines[0].transform.localPosition = new Vector3(0, 0);
        }
    }

    public void AddObstacleLine(MarbleObstacleLine obstacleLine) {
        _marbleObstacleLines.Add(obstacleLine);
        FormatObstacleLines();
    }

    private void DropperPositionUpdate() {
        _dropperDelta += Time.deltaTime * DropperMovementSpeed;
        if (_dropperDelta > 1) {
            _dropperDelta -= 1;
        }
        _marbleDropper.transform.localPosition = new Vector3(MarbleDropperMovementCurve.Evaluate(_dropperDelta) * (-_offset.x + (_marbleDropper.transform.localScale.x * 0.5f)), _offset.y);
    }
}
