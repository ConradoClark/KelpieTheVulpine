using System.Linq;
using Licht.Unity.Objects;
using UnityEngine;

public class MovingPlatform : BaseGameObject
{
    public float PlatformMultiplier;
    private FoxFormPlayer _fox;
    private MovableBySwitch _movable;
    private Collider2D _collider;

    protected override void OnAwake()
    {
        base.OnAwake();
        _fox = _fox.FromScene();
        _movable = GetComponent<MovableBySwitch>();
        _collider = _movable.GetComponent<Collider2D>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _movable.OnMove += OnMove;
    }

    private void OnMove(Vector2 obj)
    {
        if (_fox.GroundCollider.Triggers.Any(t => t.Collider == _collider))
        {
            _fox.transform.position += new Vector3(obj.x, 0);
        }
            //_fox.PhysicsObject.ApplySpeed(new Vector2(obj.x * PlatformMultiplier * (float) GameTimer.UpdatedTimeInMilliseconds,0));
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _movable.OnMove -= OnMove;
    }
}
