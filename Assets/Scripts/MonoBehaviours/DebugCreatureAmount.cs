using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class DebugCreatureAmount : MonoBehaviour {

    public static long CreatureAmount = 0;

    private TMPro.TMP_Text _text;
    private float delta = 0;
    private float updates = 1.0f/30.0f;

    private void Start() {
        _text = GetComponent<TMPro.TMP_Text>();
        CreatureAmount = 0;
    }

    private void Update() {
        delta += Time.deltaTime;
        if (delta >= updates) {
            delta -= updates;
            _text.SetText("Entities: " + CreatureAmount);
        }
    }
}
