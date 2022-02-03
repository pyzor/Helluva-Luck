﻿using UnityEngine;

public class GluttonyFreeRewardTileBehavior: WorldTileBehavior {

    private static float _probability = 1;
    public static float Probability { get { return _probability; } set { _probability = value; } }

    public override float GetProbability() {
        return _probability;
    }

    public override WorldTileBehavior Create() {
        return new GluttonyFreeRewardTileBehavior();
    }

    public override void OnAction() { }

    public override void OnHopOver() { }

    public override void OnDestroy() { }

    public override void OnEnable() { }

    public override void OnDisable() { }

    public override void Update() { }

    public override void UpdateMaterial(MeshRenderer meshRenderer) {
        meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    protected override void Init() {
        _propertyBlock.SetFloat("_TileIndex", 6);

        _propertyBlock.SetColor("_Color", ColorPalette.Orange);
        _propertyBlock.SetColor("_BorderColor", ColorPalette.DarkOrange);

        _propertyBlock.SetFloat("_TextureScale", -0.15f);

        _propertyBlock.SetFloat("_TextureAngle", -45);
        _propertyBlock.SetFloat("_RotationSpeed", 0);

        _propertyBlock.SetVector("_TextureOffset", new Vector4(0, 0.025f));
    }
}