using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationMaster : MonoBehaviour {

    public static WorldGenerationMaster Instance { get; private set; }

    [SerializeField] private GameObject WorldTilePrefab;
    [SerializeField] private int ViewDistance;

    private WorldTile _lastTile;
    private Queue<WorldTile> _tilesPool;
    private List<WorldTile> _activeTiles;

    private RandomBehaviorSystem _randomBehaviorSystem;
    
    public WorldGenerationMaster() {
        _tilesPool = new Queue<WorldTile>();
        _activeTiles = new List<WorldTile>();
        _randomBehaviorSystem = new RandomBehaviorSystem();
    }

    public void AddNewBehavior(WorldTileBehavior behavior) {
        _randomBehaviorSystem.AddEntry(behavior);
    }

    public void Init(int seed) {
        Random.InitState(seed);

        ClearActiveTiles();
        ClearTilesPool();

        WorldTile tile = Instantiate(WorldTilePrefab, transform).GetComponent<WorldTile>();
        tile.Init(null);
        tile.SetBehavior(new NormalTileBehavior());

        _lastTile = tile;
        _activeTiles.Add(_lastTile);

        FillViewDistance(_lastTile.ID);
    }

    public void CreateNextWorldTileFromPool() {
        WorldTile tile;
        if (_tilesPool.Count == 0) {
            tile = Instantiate(WorldTilePrefab, transform).GetComponent<WorldTile>();
        } else {
            tile = _tilesPool.Dequeue();
        }

        tile.Init(_lastTile);
        tile.SetBehavior(_randomBehaviorSystem.GetBehavior(GetRollValue(tile)));

        _lastTile = tile;
        _activeTiles.Add(_lastTile);
    }

    public void FillViewDistance(int id) {
        for (int i = _lastTile.ID; i < id + ViewDistance; i++) {
            CreateNextWorldTileFromPool();
        }
    }

    public void MakeFirstTile(int id) {
        RemoveTilesBelow(id);
        FillViewDistance(id);
    }

    public void RemoveTilesAbove(int id) {
        for (int i = 0; i < _activeTiles.Count; i++) {
            if (_activeTiles[i].ID == id)
                _lastTile = _activeTiles[i];
            if (_activeTiles[i].ID > id) 
                _activeTiles[i].InitiateRemoving();
        }
    }

    public void RemoveTilesBelow(int id) {
        for (int i = 0; i < _activeTiles.Count; i++) {
            if (_activeTiles[i].ID >= id)
                return;
            _activeTiles[i].InitiateRemoving();
        }
    }

    public void MoveBackToPool(WorldTile worldTile) {
        if (!_activeTiles.Remove(worldTile))
            return;

        worldTile.gameObject.SetActive(false);
        _tilesPool.Enqueue(worldTile);
    }

    public WorldTile GetTile(int id) {
        for (int i = 0; i < _activeTiles.Count; i++) {
            var worldTile = _activeTiles[i];
            if (worldTile.ID == id) {
                return worldTile;
            }
        }
        return null;
    }

    public WorldTile GetFirstTile() {
        for (int i = 0; i < _activeTiles.Count; i++) {
            var worldTile = _activeTiles[i];
            if (!worldTile.IsRemoving)
                return worldTile;
        }
        return null;
    }

    public void SetActive(bool state) {
        if (state) {
            for (int i = 0; i < _activeTiles.Count; i++) {
                if (!_activeTiles[i].IsRemoving) {
                    var tile = _activeTiles[i];
                    transform.localPosition = -new Vector3(tile.TileCoords.x, 0, tile.TileCoords.y);
                    gameObject.SetActive(state);
                    return;
                }
            }
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(Mathf.Ceil(pos.x), pos.y, Mathf.Ceil(pos.z));
        }
        gameObject.SetActive(state);
    }

    public void ClearBehaviors() {
        _randomBehaviorSystem.ClearEntries();
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void ClearActiveTiles() {
        if (_activeTiles != null) {
            for (int i = 0; i < _activeTiles.Count; i++) {
                var tile = _activeTiles[i];
                if (tile != null)
                    Destroy(tile.gameObject);
            }
            _activeTiles.Clear();
        }
    }

    private void ClearTilesPool() {
        if (_tilesPool != null) {
            for (int i = 0; i < _tilesPool.Count; i++) {
                var tile = _tilesPool.Dequeue();
                if (tile != null)
                    Destroy(tile.gameObject);
            }
            _tilesPool.Clear();
        }
    }

    void OnDestroy() {
        ClearActiveTiles();
        ClearTilesPool();
        ClearBehaviors();
    }

    private static float GetRollValue(WorldTile tile) {
        return Random.Range(0.0f, 1.0f);
    }
}
