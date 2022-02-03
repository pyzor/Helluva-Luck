using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureTest : MonoBehaviour {
    [SerializeField] private GameObject Creature;


    [SerializeField] private TMPro.TMP_Text _text;


    private int Count = 0;

    private float _preSec = 1.0f/100.0f;
    private float _delta = 0;

    [SerializeField] private float radius = 5f;

    private void CreateNewCreep() {
        Instantiate(Creature, new Vector3(Random.Range(-1.0f, 1.0f) * radius, Random.Range(-1.0f, 1.0f) * radius + transform.localPosition.y), Quaternion.identity, transform);
        Count += 1;
    }


    private void UpdateText() {
        _text.text = "Creeps " + Count;
    }

    void Update() {

        if (Count < 3000) {
            _delta += Time.deltaTime;
            if (_delta >= _preSec) {
                _delta -= _preSec;
                CreateNewCreep();
                UpdateText();
            }
        }
    }
}
