using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleGoalSpacer : MonoBehaviour {

    [SerializeField] private Transform SpikeScaler;

    [SerializeField] private SpriteRenderer BodySpriteRenderer;
    [SerializeField] private SpriteRenderer SpikeSpriteRenderer;



    public void SetSpikeScale(float value) {
        SpikeScaler.transform.localScale = new Vector3(1, Mathf.Max(value, 0.001f), 1);
    }

    public void SetColor(Color color) {
        BodySpriteRenderer.color = color;
        SpikeSpriteRenderer.color = color;
    }

    void Start() {

    }

    void Update() {

    }
}
