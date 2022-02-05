using UnityEngine;

public abstract class WorldTileBehavior {


    public abstract float GetProbability();


    protected WorldTile _worldTile;
    protected MaterialPropertyBlock _propertyBlock;

    protected WorldTileBehavior() {
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void SetWorldTile(WorldTile worldTile) {
        _worldTile = worldTile;
        Init();
    }

    public void UpdateMaterial(MeshRenderer meshRenderer) {
        meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    public abstract WorldTileBehavior Create();

    public abstract void OnAction();
    public abstract void OnHopOver();
    public abstract void OnDestroy();
    public abstract void OnEnable();
    public abstract void OnDisable();
    public abstract void Update();

    protected abstract void Init();

}
