using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class FPSUpdate : MonoBehaviour {

    private TMPro.TMP_Text _text;
    private float delta = 0;
    private float updates = 1.0f/16.0f;

    private void Start() {
        _text = GetComponent<TMPro.TMP_Text>();
    }

    private void Update() {
        delta += Time.deltaTime;
        if (delta >= updates) {
            delta -= updates;
            _text.SetText("FPS: " + Mathf.Round(1.0f / Time.unscaledDeltaTime));
        }
    }
}
