using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour {

    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private GameObject ActionBarSliderPrefab;
    [SerializeField] private AnimationCurve SliderPositionCurve;

    [SerializeField] private RectTransform _HitZonesTransform;

    [SerializeField] private GameObject ActionBarMarkPrefab;
    [SerializeField] private RectTransform _HitMarksTransform;



    private Queue<ActionBarMark> _markPool;
    private List<ActionBarMark> _activeMarks;

    private ActionBarSlider _slider;

    private List<ActionBarZone> _actionBarZones;

    private BulletsManager _bulletsManager;

    private float _sliderDelta;
    private float _sliderSpeed;

    private bool _initDone = false;

    public float Height { get { return _rectTransform.rect.height; } }
    public BulletsManager BulletsManager { get { return _bulletsManager; } }
    public RectTransform HitZonesTransform { get { return _HitZonesTransform; } }

    public void Init(BulletsManager bulletsManager, float sliderSpeed = 1) {
        _bulletsManager = bulletsManager;
        _sliderDelta = 0;
        _sliderSpeed = sliderSpeed;

        if (_slider != null) {
            Destroy(_slider.gameObject);
        }
        _slider = Instantiate(ActionBarSliderPrefab, transform).GetComponent<ActionBarSlider>();
        _slider.SetSizeDelta(1, 16);
        _slider.Init(Color.black, Color.white, 5);

        DestroyActionBarZones();
        DestroyActiveMarks();
        DestroyMarkPool();

        _initDone = true;
    }

    public void AddZone(ActionBarZone zone) {
        _actionBarZones.Add(zone);
        zone.SetSizeWidthDelta(_rectTransform.rect.width * 0.6f);
        FormatZones();
    }

    public void Hit() {
        _slider.Hit();
        Bounds markRect = CreateMark(Color.red, 2, _slider.GetLocalPosition().y);
        for(int i = 0; i < _actionBarZones.Count; i++) {
            var zone = _actionBarZones[i];
            if (markRect.Intersects(zone.Bounds)) {
                zone.Hit();
            }
        }
    }

    public void MoveBackToPool(ActionBarMark mark) {
        if (!_activeMarks.Remove(mark))
            return;

        mark.gameObject.SetActive(false);
        _markPool.Enqueue(mark);
    }

    private void FormatZones() {
        float totalHeight = 0;
        for(int i = 0; i < _actionBarZones.Count; i++) {
            totalHeight += _actionBarZones[i].TotalHeight;
        }
        float offset = 0;
        for(int i = 0; i < _actionBarZones.Count; i++) {
            var zone = _actionBarZones[i];
            float y = offset + zone.Spacing + zone.Height * 0.5f - totalHeight * 0.5f;
            zone.SetLocalPosition(0, y);
            offset += zone.TotalHeight;
        }
    }

    private Bounds CreateMark(Color color, float fadeRate, float yOffset) {
        ActionBarMark mark;

        if (_markPool.Count == 0) {
            mark = Instantiate(ActionBarMarkPrefab, _HitMarksTransform.transform).GetComponent<ActionBarMark>();
            mark.SetActionBar(this);
        } else {
            mark = _markPool.Dequeue();
        }
        mark.SetTransform(0, _slider.GetLocalPosition().y, 1, 8);
        mark.Init(color, fadeRate);

        _activeMarks.Add(mark);
        return mark.Bounds;
    }

    void Awake() {
        _actionBarZones = new List<ActionBarZone>();
        _markPool = new Queue<ActionBarMark>();
        _activeMarks = new List<ActionBarMark>();
    }

    void Update() {
        if (!_initDone)
            return;

        _sliderDelta += Time.deltaTime * _sliderSpeed;
        if (_sliderDelta >= 1) {
            _sliderDelta -= 1;
        }

        UpdateSliderPosition();
    }

    private void UpdateSliderPosition() {
        _slider.SetLocalPosition(0, SliderPositionCurve.Evaluate(_sliderDelta) * _rectTransform.rect.height * 0.5f);
    }

    private void DestroyActionBarZones() {
        if (_actionBarZones != null) {
            for (int i = 0; i < _actionBarZones.Count; i++) {
                Destroy(_actionBarZones[i].gameObject);
            }
            _actionBarZones.Clear();
        }
    }

    private void DestroyActiveMarks() {
        if (_activeMarks != null) {
            for (int i = 0; i < _activeMarks.Count; i++) {
                if (_activeMarks[i] != null)
                    Destroy(_activeMarks[i].gameObject);
            }
            _activeMarks.Clear();
        }
    }

    private void DestroyMarkPool() {
        if (_markPool != null) {
            for (int i = 0; i < _markPool.Count; i++) {
                var mark = _markPool.Dequeue();
                if (mark != null)
                    Destroy(mark.gameObject);
            }
            _markPool.Clear();
        }
    }

    private void OnDestroy() {
        DestroyActionBarZones();
        DestroyActiveMarks();
        DestroyMarkPool();
    }
}
