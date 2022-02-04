using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour {

    private enum DiceState {
        DISABLED,
        ROLLING, 
        HIT        // Stop dice and get value
    }

    /*  PROPERTY_NUMBER_ANIMATION
        X - initial number
        Y - max number
        Z - frames per second
        W - time
     */
    private static readonly int MATERIAL_PROPERTY_NUMBER_ANIMATION;
    private static readonly int MATERIAL_PROPERTY_STOP_ANIMATION;

    static DiceController() {
        MATERIAL_PROPERTY_NUMBER_ANIMATION = Shader.PropertyToID("_NumberAnimation");
        MATERIAL_PROPERTY_STOP_ANIMATION = Shader.PropertyToID("_StopAnimation");
    }

    [SerializeField] private float DiceSpeed = 6;
    [SerializeField] private float AfterHitPause = 0.5f;
    [SerializeField] private float HitAnimationSpeed = 8f;

    [SerializeField] private DiceState _currentDiceState;

    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propertyBlock;

    private float _diceTime;
    private float _diceShaderRoll;
    private int _lastDiceRoll;

    public float LastDiceRoll { get { return _lastDiceRoll; } }
    public bool IsRolling { get { return _currentDiceState == DiceState.ROLLING; } }
    public bool IsReadyToRoll { get { return _currentDiceState == DiceState.DISABLED; } }

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();

        _currentDiceState = DiceState.DISABLED;
        gameObject.SetActive(false);
    }

    public void StartRolling() {
        if (_currentDiceState != DiceState.DISABLED)
            return;

        gameObject.SetActive(true);
        _currentDiceState = DiceState.ROLLING;
        _diceTime = Time.timeSinceLevelLoad;
        _diceShaderRoll = UnityEngine.Random.Range(1, 7);

        UpdateDiceRoll();
        ApplyPropertyBlock();
    }


    public IEnumerator Hit(Action<int> onRollEnd) {
        _currentDiceState = DiceState.HIT;

        float diceAnimationDelta = 0;
        float dt = 1f / 60f;
        Vector3 scale = transform.localScale;

        while (diceAnimationDelta <= 1) { // animation
            float scaleMulty = (1 - Mathf.Sin(diceAnimationDelta * Mathf.PI) * 0.5f);
            float horizontalScaleMulty = 1 + 1 - scaleMulty;
            transform.localScale = new Vector3(scale.x * horizontalScaleMulty, scale.y * scaleMulty, scale.z * horizontalScaleMulty);

            diceAnimationDelta += dt * HitAnimationSpeed;
            yield return new WaitForSeconds(dt);
        }
        transform.localScale = scale;

        UpdateDiceRoll(Time.timeSinceLevelLoad);
        ApplyPropertyBlock();
        float timePassed = Time.timeSinceLevelLoad - _diceTime;
        _lastDiceRoll = Mathf.FloorToInt((_diceShaderRoll + (timePassed * DiceSpeed) - 1f) % 6) + 1;
        
        yield return new WaitForSeconds(AfterHitPause); // pause

        onRollEnd(_lastDiceRoll);
        _currentDiceState = DiceState.DISABLED;
        gameObject.SetActive(false);
    }

    private void ApplyPropertyBlock() {
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void UpdateDiceRoll(float stopAnimation = -1) {
        _propertyBlock.SetVector(MATERIAL_PROPERTY_NUMBER_ANIMATION, new Vector4(_diceShaderRoll, 6, DiceSpeed, _diceTime));
        _propertyBlock.SetFloat(MATERIAL_PROPERTY_STOP_ANIMATION, stopAnimation);
    }

}
