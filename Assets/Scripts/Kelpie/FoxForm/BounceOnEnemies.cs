using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class BounceOnEnemies : BaseGameObject
{
    public LayerMask LayerToCheck;
    public LichtPlatformerJumpController.CustomJumpParams BounceParams;
    public float MinJumpDelay;
    private Player _player;
    private LichtPhysics _physics;

    public class OnBounceEventHandler
    {
        public Collider2D Collider;
        public Bounceable Target;
    }

    private IEventPublisher<AbilityEvents, OnBounceEventHandler> _eventPublisher;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance();
        _eventPublisher = this.RegisterAsEventPublisher<AbilityEvents, OnBounceEventHandler>();
        _physics = this.GetLichtPhysics();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        BounceParams.MinJumpDelay = MinJumpDelay;
        DefaultMachinery.AddBasicMachine(-1, HandleBounce());
    }

    private IEnumerable<IEnumerable<Action>> HandleBounce()
    {
        while (ComponentEnabled)
        {
            Bounceable target = null;
            CollisionResult trigger = default;
            var conditions = _player.IsFox
                             && !_player.FoxForm.JumpController.IsJumping
                             && (trigger = _player.FoxForm.GroundCollider.Triggers.FirstOrDefault(t =>
                                 LayerToCheck.Contains(t.Collider.gameObject.layer))).TriggeredHit
                             && _physics.TryGetPhysicsObjectByCollider(trigger.Collider, out var physicsObject)
                             && physicsObject.TryGetCustomObject(out target)
                             && target.enabled;
            if (conditions)
            {
                _eventPublisher.PublishEvent(AbilityEvents.OnBounce, new OnBounceEventHandler
                {
                    Collider = trigger.Collider,
                    Target = target
                });
                yield return _player.FoxForm.JumpController.ExecuteJump(customParams: BounceParams).AsCoroutine();
            }
            else yield return TimeYields.WaitOneFrameX;
        }
    }
}
