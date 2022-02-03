using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarMark : MonoBehaviour {

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _imageComponent;

    private float _fadeRate = 1;
    private float _fadeDelta = 1;

    private ActionBar _actionBar;
    private Color _initialColor;
    private Color _fadedColor;


    public Bounds Bounds {
        get {
            return new Bounds(_rectTransform.localPosition, new Vector3(_rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y, 1));
        }
    }

    public void SetActionBar(ActionBar actionBar) {
        _actionBar = actionBar;
    }

    public void Init(Color color, float fadeRate = 1) {
        _initialColor = color;
        _imageComponent.color = _initialColor;

        _fadedColor = _initialColor;
        _fadedColor.a = 0;
        _fadeRate = fadeRate;
        _fadeDelta = 1;

        gameObject.SetActive(true);
    }

    public void SetTransform(float x, float y, float width, float height) {
        _rectTransform.localPosition = new Vector3(x, y);
        _rectTransform.sizeDelta = new Vector2(width, height);
    }

    private void MoveBackToPool() {
        _actionBar.MoveBackToPool(this);
    }

    private void UpdateColor() {
        _imageComponent.color = Color.Lerp(_fadedColor, _initialColor, _fadeDelta);
    }

    void Update() {
        _fadeDelta -= Time.deltaTime * _fadeRate;
        _fadeDelta = Mathf.Max(_fadeDelta, 0);
        UpdateColor();
        if(_fadeDelta == 0) {
            MoveBackToPool();
        }
    }

}
