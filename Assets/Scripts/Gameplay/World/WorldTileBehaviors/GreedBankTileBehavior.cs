using UnityEngine;

public class GreedBankTileBehavior : WorldTileBehavior {

    private static float _probability = 1;
    public static float Probability { get { return _probability; } set { _probability = value; } }

    public override float GetProbability() {
        return _probability;
    }

    private ParticleSystem _particleSystem;

    public override WorldTileBehavior Create() {
        return new GreedBankTileBehavior();
    }

    public override void OnAction() {
        GameSession.Instance.BankDeposit *= 2;
        BankDialogue.Instance.Show();
    }

    public override void OnHopOver() {
        if (GameSession.Instance.BankDeposit > 0) {
            _particleSystem.Play(true);
            GameSession.Instance.BankDeposit = 0;
        }
    }

    public override void OnDestroy() {
        if (_particleSystem != null)
            Object.Destroy(_particleSystem.gameObject);
    }

    public override void OnEnable() { }

    public override void OnDisable() { }

    public override void Update() { }

    protected override void Init() {
        _particleSystem = Object.Instantiate(WorldTileResources.Instance.GetTileParticles("bankTile_miss"), _worldTile.transform);
        _particleSystem.transform.localPosition = Vector3.up * 0.5f;

        _propertyBlock.SetFloat("_TileIndex", 3);

        _propertyBlock.SetColor("_Color", ColorPalette.Golden);
        _propertyBlock.SetColor("_BorderColor", ColorPalette.Black);

        _propertyBlock.SetFloat("_TextureScale", -0.1f);

        _propertyBlock.SetFloat("_TextureAngle", -45);
        _propertyBlock.SetFloat("_RotationSpeed", 0);

        _propertyBlock.SetVector("_TextureOffset", new Vector4(-0.0125f, 0));
    }
}

