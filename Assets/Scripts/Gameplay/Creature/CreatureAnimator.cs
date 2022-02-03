using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

using Random = UnityEngine.Random;

public class CreatureAnimator : MonoBehaviour {

    public static readonly float2 STATE_IDLE;
    public static readonly float2 STATE_WALK;
    public static readonly float2 STATE_ATTACK;

    public static readonly float CLASS_TYPE_Y_OFFSET;

    private static int MATERIAL_PROPERTY_ANIMATION;
    private static int MATERIAL_PROPERTY_FLIP_X;
    private static int MATERIAL_PROPERTY_TINT_COLOR;


    static CreatureAnimator() {
        STATE_IDLE = new float2(0, 2);
        STATE_WALK = new float2(1, 6);
        STATE_ATTACK = new float2(2, 4);

        CLASS_TYPE_Y_OFFSET = 4;

        MATERIAL_PROPERTY_ANIMATION = Shader.PropertyToID("_Animation");
        MATERIAL_PROPERTY_FLIP_X = Shader.PropertyToID("_FlipUVX");
        MATERIAL_PROPERTY_TINT_COLOR = Shader.PropertyToID("_TintColor");
    }

    [SerializeField] private CreatureClass _creatureClass;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private MaterialPropertyBlock _materialPropertyBlock;

    [SerializeField] private float _animationSpeed = 8;

    /// <summary>
    /// </summary>
    /// <param name="newState">New Animation STATE_...</param>
    /// <param name="multiplier">Animation speed multiplier</param>
    public void ChangeAnimationState(float2 newState, float multiplier = 1f) {
        _materialPropertyBlock.SetVector(MATERIAL_PROPERTY_ANIMATION, BuildAnimationProperty(newState, multiplier));
    }

    public void SetColor(Color c) {
        _materialPropertyBlock.SetColor(MATERIAL_PROPERTY_TINT_COLOR, c);
    }

    public void LookLeft(bool state){
        _materialPropertyBlock.SetFloat(MATERIAL_PROPERTY_FLIP_X, state ? 1f : 0f);
    }

    public void ApplyMaterialBlock() {
        _spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
    }


    private void Awake() {
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    private float4 BuildAnimationProperty(float2 state, float multiplier) {
        return new float4(state.x + CLASS_TYPE_Y_OFFSET * (int)_creatureClass.ClassType, state.y, _animationSpeed * multiplier, Time.timeSinceLevelLoad);
    }
}
