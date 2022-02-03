using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightControllerTest : MonoBehaviour {

    private FightController fightController;

    private void Awake() {
        fightController = GetComponent<FightController>();
    }

    public void Test() {
        fightController.BeginFight(0, 0);
    }
}
