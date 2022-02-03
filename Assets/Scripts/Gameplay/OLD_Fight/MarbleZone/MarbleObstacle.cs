using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MarbleObstacle : MonoBehaviour {

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Rigidbody2D _rigidbody2D;

    private MarbleObstacleLine _obstacleLine;
    private MarbleObstacleBehavior _behavior;

    private Color _color;
    private float _angularVelocity = 0;

    public MarbleObstacleLine ObstacleLine { get { return _obstacleLine; } }
    public Color Color { get { return _color; } }

    public void SetBehavior(MarbleObstacleBehavior behavior) {
        _behavior = behavior;
        _behavior.Init();
    }

    public void Init(MarbleObstacleLine obstacleLine, Color color, float angularVelocity = 0) {
        _obstacleLine = obstacleLine;
        _color = color;
        _spriteRenderer.color = _color;
        SetAngularVelocity(angularVelocity);
    }

    public void SetColor(Color color) {
        _spriteRenderer.color = color;
    }

    public void ResetColor() {
        _spriteRenderer.color = _color;
    }

    public void SetAngularVelocity(float angularVelocity) {
        _angularVelocity = angularVelocity;
        UpdateVelocity();
    }

    public void SetAngle(float angle) {
        var euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(euler.x, euler.y, angle));
    }

    private void UpdateVelocity() {
        _rigidbody2D.angularVelocity = _angularVelocity;
    }

    private void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        _behavior.OnCollision(collision);
        // todo work behavior
    }

    void Start() {

    }

    void Update() {
        _behavior.Update();
    }
}
