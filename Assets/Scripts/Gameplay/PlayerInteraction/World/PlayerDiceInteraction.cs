using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiceInteraction : MonoBehaviour {

    [SerializeField] private PlayerWorldMovement PlayerWorldMovement;
    [SerializeField] private DiceController DiceController;

    [SerializeField] private float DiceJumpSpeed = 2;
    [SerializeField] private float DiceJumpHeight = 0.6f;

    private void Awake() {
        if (PlayerWorldMovement == null)
            PlayerWorldMovement = GameObject.Find("Player").GetComponent<PlayerWorldMovement>();
        if (DiceController == null)
            DiceController = GameObject.Find("Dice").GetComponent<DiceController>();
    }

    private bool _coroutineExist = false;

    public void HitTheDice() {
        if(DiceController.IsRolling && !_coroutineExist)
            StartCoroutine(DiceJump());
    }

    private IEnumerator DiceJump() {
        _coroutineExist = true;
        bool hitPerformed = false;

        float lastTime = Time.realtimeSinceStartup;
        float jump_dt = 0, currentTime = 0;

        while (jump_dt <= 1) {
            currentTime = Time.realtimeSinceStartup;
            jump_dt += (currentTime - lastTime) * DiceJumpSpeed;
            lastTime = currentTime;

            if (jump_dt >= 0.5f && !hitPerformed && DiceController.IsRolling) {
                hitPerformed = true;
                StartCoroutine(DiceController.Hit((int roll) => {
                    PlayerWorldMovement.MovePlayer(roll);
                }));
            }

            PlayerWorldMovement.transform.localPosition = new Vector3(0, 0.3f + DiceJumpHeight * Mathf.Sin(Mathf.PI * jump_dt), 0);

            yield return null;
        }
        _coroutineExist = false;
    }

    void Start() {

    }

    void Update() {
        if(DiceController.IsReadyToRoll && !PlayerWorldMovement.IsMoving()) {
            DiceController.StartRolling();
        }
        
    }


}
