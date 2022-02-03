using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiceInputHandler : MonoBehaviour {

    [SerializeField] private PlayerWorldMovement PlayerWorldMovement;
    [SerializeField] private DiceController DiceController;

    private void Awake() {
        if (PlayerWorldMovement == null)
            PlayerWorldMovement = GameObject.Find("Player").GetComponent<PlayerWorldMovement>();
        if (DiceController == null)
            DiceController = GameObject.Find("Dice").GetComponent<DiceController>();
    }

    private bool _coroutineStarted = false;

    public void HitTheDice() {
        if(DiceController.DiceRolling && !_coroutineStarted)
            StartCoroutine(DiceJump());
    }

    private IEnumerator DiceJump() {
        _coroutineStarted = true;
        bool hitPerformed = false;

        float lastTime = Time.realtimeSinceStartup;
        float jump_dt = 0, currentTime = 0;

        while (jump_dt <= 1) {
            currentTime = Time.realtimeSinceStartup;
            jump_dt += (currentTime - lastTime) * PlayerWorldMovement.GetDiceJumpSpeed();
            lastTime = currentTime;

            if(jump_dt >= 0.5f && !hitPerformed) {
                hitPerformed = true;
                DiceController.Hit();
            }
            PlayerWorldMovement.DiceJump(jump_dt);

            yield return null;
        }
        _coroutineStarted = false;
    }

    void Start() {

    }

    void Update() {
        if(DiceController.DiceEnabled && DiceController.IsReadyToRoll() && !PlayerWorldMovement.IsMoving()) {
            DiceController.gameObject.SetActive(true);
            DiceController.StartRolling();
        }
        
    }


}
