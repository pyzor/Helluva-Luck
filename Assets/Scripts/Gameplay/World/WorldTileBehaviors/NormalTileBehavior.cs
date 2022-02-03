using UnityEngine;

public class NormalTileBehavior : WorldTileBehavior {

    private static float _probability = 1;
    public static float Probability { get { return _probability; } set { _probability = value; } }

    public override float GetProbability() {
        return _probability;
    }


    public override WorldTileBehavior Create() {
        return new NormalTileBehavior();
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
        _propertyBlock.SetFloat("_TileIndex", 0);

        _propertyBlock.SetColor("_Color", ColorPalette.Brown);
        _propertyBlock.SetColor("_BorderColor", ColorPalette.DarkGrey);

        _propertyBlock.SetFloat("_TextureScale", 0.2928932f);

        _propertyBlock.SetFloat("_TextureAngle", Random.Range(0.0f, 360.0f));
        _propertyBlock.SetFloat("_RotationSpeed", 0);

        _propertyBlock.SetVector("_TextureOffset", Vector4.zero);
    }
}
