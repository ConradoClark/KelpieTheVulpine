using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Effects;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAttack : BaseGameObject
{
    public float DashSpeed;
    public InputActionReference DashInput;
    public InputActionReference MouseClick;
    public TintFlash DashFlash;
    public GhostFollowMouse MouseFollow;
    public ScriptPrefab TrailEffect;
    public SpriteRenderer DashEffect;
    public LichtPhysicsCollisionDetector CollisionDetector;

    private GhostFormPlayer _player;
    private PrefabPool _trailPool;
    private bool _knockedBack;
    private LichtPhysics _physics;

    public bool IsDashing { get; private set; }

    public class OnDashEventHandler
    {
        public Collider2D Collider;
        public Dasheable Target;
        public Vector2 Speed;
    }

    private IEventPublisher<AbilityEvents, OnDashEventHandler> _eventPublisher;


    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance().GhostForm;
        _trailPool = SceneObject<EffectsManager>.Instance().GetEffect(TrailEffect);
        _physics = this.GetLichtPhysics();
        _eventPublisher = this.RegisterAsEventPublisher<AbilityEvents, OnDashEventHandler>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleDash());
    }

    private IEnumerable<IEnumerable<Action>> HandleTrail()
    {
        while (IsDashing)
        {
            if (_trailPool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position;
            }

            yield return TimeYields.WaitMilliseconds(GameTimer, 25);
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleDashCollision()
    {
        Dasheable dasheable = null;
        while (IsDashing)
        {
            var target = CollisionDetector.Triggers.FirstOrDefault(t =>
                t.TriggeredHit &&
                _physics.TryGetPhysicsObjectByCollider(t.Collider, out var physicsObject) &&
                physicsObject.TryGetCustomObject(out dasheable));

            if (target.TriggeredHit && (dasheable?.ComponentEnabled ?? false))
            {
                _eventPublisher.PublishEvent(AbilityEvents.OnDash, new OnDashEventHandler
                {
                    Collider = target.Collider,
                    Target = dasheable,
                    Speed = _player.PhysicsObject.LatestNonZeroSpeed,
                });

                IsDashing = false;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleDash()
    {
        while (ComponentEnabled)
        {
            if (DashInput.action.WasPerformedThisFrame())
            {
                _player.MoveController.BlockMovement(this);
                MouseFollow.BlockMovement(this);

                IsDashing = true;
                DashEffect.enabled = true;
                _knockedBack = false;

                DefaultMachinery.AddBasicMachine(HandleTrail());
                DefaultMachinery.AddBasicMachine(HandleDashCollision());

                var latestDirection = MouseClick.action.WasPerformedThisFrame() ?
                    MouseFollow.GetDirectionFromMouse()
                    : _player.MoveController.LatestDirection;

                var move = _player.PhysicsObject.GetSpeedAccessor(latestDirection * DashSpeed)
                    .ToSpeed(Vector2.zero)
                    .Over(0.5f)
                    .BreakIf(()=> _knockedBack || !ComponentEnabled || !IsDashing, false)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(GameTimer)
                    .Build();

                if (_knockedBack)
                {
                    var currentSpeed = _player.PhysicsObject.LatestNonZeroSpeed;
                    yield return _player.PhysicsObject.GetSpeedAccessor(-currentSpeed * 0.5f)
                        .ToSpeed(Vector2.zero)
                        .Over(0.2f)
                        .BreakIf(() => !ComponentEnabled || !IsDashing, true)
                        .Easing(EasingYields.EasingFunction.CubicEaseOut)
                        .UsingTimer(GameTimer)
                        .Build();
                }

                yield return DashFlash.Flash().AsCoroutine().Combine(move);
                _player.MoveController.UnblockMovement(this);
                MouseFollow.UnblockMovement(this);
                
                DashEffect.enabled = false;
                IsDashing = false;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
