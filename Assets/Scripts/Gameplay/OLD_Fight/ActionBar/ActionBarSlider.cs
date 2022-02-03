using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarSlider : MonoBehaviour {

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _imageComponent;


    private Color _mainColor;
    private Color _hitColor;

    private float _fadeRate;
    private float _fadeDelta;

    private bool _hitFade;

    public void Init(Color mainColor, Color hitColor, float fadeRate = 1) {
        _mainColor = mainColor;
        _hitColor = hitColor;
        _imageComponent.color = _mainColor;

        _fadeRate = fadeRate;
        _fadeDelta = 0;
        _hitFade = false;
    }

    public void Hit() {
        _fadeDelta = 1;
        _hitFade = true;
    }

    public void SetLocalPosition(float x, float y) {
        _rectTransform.localPosition = new Vector3(x, y);
    }
    
    public Vector3 GetLocalPosition() {
        return _rectTransform.localPosition;
    }


    public void SetSizeDelta(float width, float height) {
        _rectTransform.sizeDelta = new Vector2(width, height);
    }

    void Start() {

    }

    private void UpdateColor() {
        _imageComponent.color = Color.Lerp(_mainColor, _hitColor, _fadeDelta);
    }

    void Update() {
        if (_hitFade) {
            _fadeDelta -= Time.deltaTime * _fadeRate;
            _fadeDelta = Mathf.Max(_fadeDelta, 0);
            UpdateColor();
            if (_fadeDelta == 0) {
                _hitFade = false;
            }
        }
    }
}
