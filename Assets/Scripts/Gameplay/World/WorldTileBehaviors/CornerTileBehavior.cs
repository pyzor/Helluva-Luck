using UnityEngine;

public class CornerTileBehavior : WorldTileBehavior {

    private static float _probability = 1;
    public static float Probability { get { return _probability; } set { _probability = value; } }

    public override float GetProbability() {
        return _probability;
    }

    private float _rotationAngle = 0;

    public override WorldTileBehavior Create() {
        return new CornerTileBehavior();
    }

    public override void OnAction() { }

    public override void OnHopOver() { }

    public override void OnDestroy() { }

    public override void OnEnable() { }

    public override void OnDisable() { }

    public override void Update() { }

    protected override void Init() {
        _worldTile.ChangeDeviation();
        _rotationAngle = Vector2.SignedAngle(Vector2.up, _worldTile.PathDirection);

        _propertyBlock.SetFloat("_TileIndex", 1);

        _propertyBlock.SetColor("_Color", ColorPalette.Brown);
        _propertyBlock.SetColor("_BorderColor", ColorPalette.DarkGrey);

        _propertyBlock.SetFloat("_TextureScale", 0);

        _propertyBlock.SetFloat("_TextureAngle", _rotationAngle);
        _propertyBlock.SetFloat("_RotationSpeed", 0);

        _propertyBlock.SetVector("_TextureOffset", Vector4.zero);
    }
}
