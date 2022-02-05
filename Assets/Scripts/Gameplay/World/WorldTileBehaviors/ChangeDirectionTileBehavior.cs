using UnityEngine;

public class ChangeDirectionTileBehavior : WorldTileBehavior {

    private static float _probability = 1;
    public static float Probability { get { return _probability; } set { _probability = value; } }

    public override float GetProbability() {
        return _probability;
    }

    private ParticleSystem _particleSystem;

    public override WorldTileBehavior Create() {
        return new ChangeDirectionTileBehavior();
    }

    public override void OnAction() {
        _worldTile.ChangeDeviation();
        WorldGenerationMaster.Instance.RemoveTilesAbove(_worldTile.ID);

        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public override void OnHopOver() {
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public override void OnDestroy() {
        if(_particleSystem != null)
            UnityEngine.Object.Destroy(_particleSystem.gameObject);
    }

    public override void OnEnable() {
        _particleSystem.Play(true);
    }

    public override void OnDisable() {
    }

    public override void Update() { }
    
    protected override void Init() {
        _particleSystem = UnityEngine.Object.Instantiate(WorldTileResources.Instance.GetTileParticles("changeDirTile_idle"), _worldTile.transform);
        _particleSystem.transform.localPosition = Vector3.up * 0.01f;

        _propertyBlock.SetFloat("_TileIndex", 2);

        _propertyBlock.SetColor("_Color", ColorPalette.Red);
        _propertyBlock.SetColor("_BorderColor", ColorPalette.DarkRed);

        _propertyBlock.SetFloat("_TextureScale", 0.2928932f);

        _propertyBlock.SetFloat("_TextureAngle", 0);
        _propertyBlock.SetFloat("_RotationSpeed", -180);

        _propertyBlock.SetVector("_TextureOffset", Vector4.zero);
    }
}