using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarZone : MonoBehaviour {

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _imageComponent;


    private Color _mainColor;
    private float _height, _spacing;

    private ActionBarZoneBehavior _behavior;
    private ActionBar _actionBar;

    public ActionBar ActionBar { get { return _actionBar; } }

    public float Height { get { return _height; } }
    public float Spacing { get { return _spacing; } }
    public float TotalHeight { get { return _height + _spacing * 2.0f; } }
    public Bounds Bounds {
        get { 
            return new Bounds(_rectTransform.localPosition, new Vector3(_rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y, 1)); 
        } 
    }

    public void Init(ActionBar actionBar, Color mainColor, float height, float spacing = 0) {
        _actionBar = actionBar;
        _mainColor = mainColor;
        _imageComponent.color = _mainColor;
        _height = height;
        _rectTransform.sizeDelta = new Vector2(1, height);
        _spacing = spacing;
        _actionBar.AddZone(this);
    }

    public void Hit() { // todo pass slider
        _behavior.Action();
    }


    public void SetBehavior(ActionBarZoneBehavior behavior) {
        _behavior = behavior;
    }

    public void SetLocalPosition(float x, float y) {
        _rectTransform.localPosition = new Vector3(x, y);
    }

    public void SetSizeWidthDelta(float width) {
        _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
    }

    void Start() {

    }

    void Update() {
        _behavior.Update();
    }
}
