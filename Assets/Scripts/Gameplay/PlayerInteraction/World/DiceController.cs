using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour {


    //enum
    private enum DiceState {
        IDLE,
        ROLLING,    // Rolling 
        HIT,        // Stop dice and get value
        DISABLED
    }


    //fields
    [SerializeField] private PlayerWorldMovement PlayerWorldMovement;
    [SerializeField] private float DiceSpeed = 6;
    [SerializeField] private float AfterHitPause = 1;
    //[SerializeField] private float HitAnimationSpeed = 5f;

    private DiceState _currentDiceState;

    private float _diceDelta;
    private float _lastDiceRoll;
    private float _diceShaderRoll;

    public float LastDiceRoll { get { return _lastDiceRoll; } }
    public bool DiceEnabled {
        get { return _currentDiceState != DiceState.DISABLED; }
        set {
            if (value) {
                if(_currentDiceState == DiceState.DISABLED) {
                    _currentDiceState = DiceState.IDLE;
                }
            } else {
                _currentDiceState = DiceState.DISABLED;
                _diceDelta = 0;
                // TODO Reset transform 
            }
        }
    }
    public bool DiceRolling { get { return _currentDiceState == DiceState.ROLLING; } }

    //methods
    private void Awake() {
        if(PlayerWorldMovement == null)
            PlayerWorldMovement = GameObject.Find("Player").GetComponent<PlayerWorldMovement>();
    }

    void Start() {
        _currentDiceState = DiceState.IDLE;
    }

    public bool IsReadyToRoll() {
        if (_currentDiceState == DiceState.IDLE)
            return true;
        return false;
    }

    public void StartRolling() {
        _currentDiceState = DiceState.ROLLING;
        _diceDelta = 0;
        _diceShaderRoll = 1;
        UpdateDiceRoll();
    }

    public void Hit() {
        _currentDiceState = DiceState.HIT;
        _lastDiceRoll = _diceShaderRoll;
        _diceDelta = 0;
    }

    private void UpdateDiceRoll() {
        GetComponent<MeshRenderer>().material.SetFloat("_Number", _diceShaderRoll);
    }

    void Update() {
        switch(_currentDiceState) {
            case DiceState.IDLE: {
                // go
                return;
            }
            case DiceState.ROLLING: {
                _diceDelta += Time.deltaTime * DiceSpeed;
                if(_diceDelta >= 6) {
                    _diceDelta -= 6;
                }
                float roll = Mathf.Floor(_diceDelta) + 1;
                if(_diceShaderRoll != roll) {
                    _diceShaderRoll = roll;
                    UpdateDiceRoll();
                }
                break;
            }
            case DiceState.HIT: {
                // hit animation

                _diceDelta += Time.deltaTime;

                //animation uses Mathf.Min(_diceDelta * HitAnimationSpeed, 1);

                if(_diceDelta >= AfterHitPause) {
                    PlayerWorldMovement.MovePlayer((int)_lastDiceRoll);
                    _currentDiceState = DiceState.IDLE;
                    gameObject.SetActive(false); // TODO
                }

                break;
            }
        }

    }
}
