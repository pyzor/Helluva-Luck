using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSUnlock : MonoBehaviour {

    [SerializeField] private int TargetFrameRate = 60;

    public void Awake() {
        Application.targetFrameRate = TargetFrameRate;
    }
}
