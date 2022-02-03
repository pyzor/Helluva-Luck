using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EdgeCollider2D))]
public class BattleGrid : MonoBehaviour {

    [SerializeField] private GameObject GridCellPrefab;
    [SerializeField] private GameObject BattleTowerPrefab;

    [SerializeField] private Sprite BackgroundSprite;
    [SerializeField] private Material BackgroundMaterial;


    private int[,] _grid;
    private float _cellSize;

    private List<SpriteRenderer> _sprites;
    private EdgeCollider2D _edgeCollider;
    private Vector3 _offset;

    private BattleTower _redTower = null;
    private BattleTower _greenTower = null;

    private GameObject _background;
    private BulletsManager _bulletsManager;

    public float CellSize { get { return _cellSize; } }
    public Vector3 Offset { get { return _offset; } }

    public void InitGrid(BulletsManager bulletsManager, float width, float height, float verticalCellsCount) {
        _bulletsManager = bulletsManager;
        for (int i = 0; i < _sprites.Count; i++) {
            Destroy(_sprites[i].gameObject);
        }
        _sprites.Clear();

        _cellSize = height / verticalCellsCount;
        int xLen = (int)(width / _cellSize);
        int yLen = (int)(height / _cellSize);

        _offset = new Vector3((-xLen * 0.5f) * _cellSize, (-yLen * 0.5f) * _cellSize, 0);

        // Grid Setup
        _grid = new int[xLen, yLen];
        
        for (int x = 0; x < xLen; x++) {  // _grid.GetLength(0)
            for (int y = 0; y < yLen; y++) { // _grid.GetLength(1)
                var gridCellGameObject = Instantiate(GridCellPrefab, transform);
                gridCellGameObject.transform.localPosition = GetCellPosition(x,y);
                gridCellGameObject.transform.localScale = new Vector3(_cellSize, _cellSize, 1);

                var gridCell = gridCellGameObject.GetComponent<BattleGridCell>();
                gridCell.Init(x, y, this);

                var spriteRenderrer = gridCellGameObject.GetComponent<SpriteRenderer>();
                if(y >= yLen / 2) { // int
                    _grid[x, y] = 1;
                    spriteRenderrer.color = GenerateShade(Color.red, 0.25f);
                } else {
                    _grid[x, y] = 0;
                    spriteRenderrer.color = GenerateShade(Color.green, 0.25f);
                }

                _sprites.Add(spriteRenderrer);
            }
        }

        // Edge Collider
        Vector2 p = new Vector2(_offset.x, _offset.y);
        _edgeCollider.SetPoints(new List<Vector2>() {
            new Vector2(-p.x, p.y),
            new Vector2(p.x, p.y),
            new Vector2(p.x, -p.y),
            new Vector2(-p.x, -p.y),
            new Vector2(-p.x, p.y),
        });

        // Towers
        if (_redTower != null) {
            Destroy(_redTower.gameObject);
        }
        if (_greenTower != null) {
            Destroy(_greenTower.gameObject);
        }

        _redTower = Instantiate(BattleTowerPrefab, transform).GetComponent<BattleTower>();
        _greenTower = Instantiate(BattleTowerPrefab, transform).GetComponent<BattleTower>();

        _redTower.SetBattleGrid(this);
        _greenTower.SetBattleGrid(this);

        float towerScale = _cellSize * 0.8f;

        _redTower.SetColor(Color.red, 1);
        _redTower.transform.localPosition = new Vector3(0, -_offset.y - towerScale/2.0f);
        _redTower.transform.localScale = new Vector3(towerScale, towerScale, 1);
        _redTower.transform.rotation = Quaternion.Euler(0, 0, 180);
        _redTower.ConsumeAvailableBullet = () => _bulletsManager.ConsumeEnemyBullet();

        _greenTower.SetColor(Color.green, 0);
        _greenTower.transform.localPosition = new Vector3(0, _offset.y + towerScale / 2.0f);
        _greenTower.transform.localScale = new Vector3(towerScale, towerScale, 1);
        _greenTower.ConsumeAvailableBullet = () => _bulletsManager.ConsumePlayerBullet();

        if (_background != null) {
            Destroy(_background);
        }

        _background = new GameObject("Background", typeof(SpriteRenderer));
        Transform gbTransform = _background.transform;
        gbTransform.SetParent(gameObject.transform, false);
        gbTransform.localPosition = Vector3.zero;
        gbTransform.localScale = new Vector3(width, height, 1);
        SpriteRenderer bgSpriteRenderer = _background.GetComponent<SpriteRenderer>();
        bgSpriteRenderer.material = BackgroundMaterial;
        bgSpriteRenderer.sprite = BackgroundSprite;
        bgSpriteRenderer.sortingOrder = -70;
        bgSpriteRenderer.color = new Color(0.1171875f, 0.1171875f, 0.1171875f);
    }
    public Vector3 GetCellPosition(int x, int y) {
        return new Vector3(x + 0.5f, y + 0.5f, 0) * _cellSize + _offset;
    }
    
    public int GetCellValue(int x, int y) {
        return _grid[x, y];
    }

    public void CaptureCell(int x, int y, int value, Color color) {
        _grid[x, y] = value;
        _sprites[y + x * _grid.GetLength(1)].color = GenerateShade(color, 0.25f);
    }

    private Color GenerateShade(Color color, float maxOffset) {
        maxOffset = Mathf.Clamp(maxOffset, 0, 1);
        float offset = Random.Range(1 - maxOffset, 1);
        color.r *= offset;
        color.g *= offset;
        color.b *= offset;
        return color;
    }

    private void Awake() {
        _sprites = new List<SpriteRenderer>();
        _edgeCollider = GetComponent<EdgeCollider2D>();
    }

    void Start() {

    }

    void Update() {
    }

}
