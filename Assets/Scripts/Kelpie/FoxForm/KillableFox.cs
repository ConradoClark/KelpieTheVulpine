using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Effects;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics.Forces;
using Licht.Unity.Pooling;
using UnityEngine;

public class KillableFox : Killable
{
    public float InvincibilityTimeInSeconds;
    public Vector2 KnockBackSpeed;
    public float KnockBackTimeInSeconds;
    public AnimationCurve KnockBackXCurve;
    public AnimationCurve KnockBackYCurve;
    public ScriptPrefab SmokeEffect;
    private Player _player;
    private FoxFormPlayer _fox;
    private GhostFormPlayer _ghost;
    private DarkWorldController _darkWorldController;
    private KelpieDeathLoop _deathLoop;
    private bool _spawnSmoke;
    private PrefabPool _smokePool;
    private IEventPublisher<StateEvents> _eventPublisher;
    private bool _isInvulnerable;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent(StateEvents.OnInvincibilityFrames, OnInvincibility);
    }

    private void OnInvincibility()
    {
        _isInvulnerable = true;
        DefaultMachinery.AddBasicMachine(HandleInvincibilityFrames());
    }

    private IEnumerable<IEnumerable<Action>> HandleInvincibilityFrames()
    {
        DefaultMachinery.AddBasicMachine(Blink());
        yield return TimeYields.WaitSeconds(GameTimer, InvincibilityTimeInSeconds);
        _isInvulnerable = false;
    }

    private IEnumerable<IEnumerable<Action>> Blink()
    {
        while (_isInvulnerable)
        {
            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            _fox.MainSpriteRenderer.enabled = false;
            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            _fox.MainSpriteRenderer.enabled = true;
        }
        _fox.MainSpriteRenderer.enabled = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent(StateEvents.OnInvincibilityFrames, OnInvincibility);
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance(true);
        _fox = _player.FoxForm;
        _ghost = _player.GhostForm;
        _darkWorldController = SceneObject<DarkWorldController>.Instance();
        _deathLoop = SceneObject<KelpieDeathLoop>.Instance(true);
        _smokePool = SceneObject<EffectsManager>.Instance().GetEffect(SmokeEffect);
        _eventPublisher = this.RegisterAsEventPublisher<StateEvents>();
    }

    protected override bool CanBeKilled()
    {
        return !_isInvulnerable;
    }

    protected override IEnumerable<IEnumerable<Action>> OnDeathEffect()
    {
        DefaultMachinery.AddBasicMachine(_darkWorldController.GoToDarkWorld());
        _fox.MoveController.BlockMovement(this);
        _fox.JumpController.BlockMovement(this);
        yield return BounceKnockBack().AsCoroutine();
        _fox.gameObject.SetActive(false);

        _fox.MoveController.UnblockMovement(this);
        _fox.JumpController.UnblockMovement(this);

        _deathLoop.Show();

        _player.CurrentForm = Player.PlayerForms.Ghost;
        _eventPublisher.PublishEvent(StateEvents.OnGhostForm);
        _ghost.transform.position = _fox.transform.position;

        _spawnSmoke = true;
        DefaultMachinery.AddBasicMachine(SpawnSmoke());
        yield return _ghost.transform.GetAccessor()
            .Position
            .ToPosition(Checkpoint.CurrentCheckpointPosition)
            .Over(Vector2.Distance(_ghost.transform.position,Checkpoint.CurrentCheckpointPosition) > 15f
                ? 4 : 2.5f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(GameTimer)
            .Build();
        _spawnSmoke = false;

        _ghost.gameObject.SetActive(true);
    }

    private IEnumerable<IEnumerable<Action>> SpawnSmoke()
    {
        while (_spawnSmoke)
        {
            if (_smokePool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = _ghost.transform.position;
            }
            yield return TimeYields.WaitMilliseconds(GameTimer, 20);
        }
    }

    private IEnumerable<IEnumerable<Action>> BounceKnockBack()
    {
        var speed = new Vector2(KnockBackSpeed.x * -_fox.MoveController.LatestDirection, KnockBackSpeed.y);

        var xKnockBack = _fox.PhysicsObject.GetSpeedAccessor(speed)
            .X
            .SetTarget(0)
            .Over(KnockBackTimeInSeconds)
            .WithAnimationCurve(KnockBackXCurve)
            .UsingTimer(GameTimer)
            .Build();

        var yKnockBack = _fox.PhysicsObject.GetSpeedAccessor(speed)
            .Y
            .SetTarget(0)
            .Over(KnockBackTimeInSeconds)
            .WithAnimationCurve(KnockBackYCurve)
            .UsingTimer(GameTimer)
            .Build();

       yield return xKnockBack.Combine(yKnockBack);
    }
}
