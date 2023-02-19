using System;
using Licht.Unity.Objects;      
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Effects;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using UnityEngine;

public class SwitchObject : BaseGameObject
{
    public Bounceable Bounceable;
    public Dasheable Dasheable;
    public SpriteRenderer SwitchSprite;
    public ScriptPrefab InteractionEffect;
    public Vector3 EffectOffset;
    public AnimationCurve ActivationAnim;

    public event Action OnActivation;
    public event Action OnReset;

    private PrefabPool _prefabPool; 
    private Collider2D _collider;
    private IEventPublisher<StateEvents> _eventPublisher;

    protected override void OnAwake()
    {
        base.OnAwake();
        _prefabPool = SceneObject<EffectsManager>.Instance().GetEffect(InteractionEffect);
        _collider = GetComponentInChildren<Collider2D>();
        _eventPublisher = this.RegisterAsEventPublisher<StateEvents>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, OnBounce);
        this.ObserveEvent<AbilityEvents, DashAttack.OnDashEventHandler>(AbilityEvents.OnDash, OnDash);
    }

    public void ResetSwitch()
    {
        if (_collider.enabled) return;

        OnReset?.Invoke();
        _collider.enabled = true;
        Dasheable.enabled = true;
        Bounceable.enabled = true;

        var resetPos = SwitchSprite.transform.GetAccessor()
            .LocalPosition
            .Y
            .Increase(0.185f)
            .Over(0.5f)
            .WithAnimationCurve(ActivationAnim)
            .UsingTimer(GameTimer)
            .Build();

        DefaultMachinery.AddBasicMachine(resetPos);
    }

    private void OnDash(DashAttack.OnDashEventHandler obj)
    {
        if (obj.Target != Dasheable) return;

        obj.Collider.enabled = false;
        DefaultMachinery.AddBasicMachine(HandleActivation());
    }

    private void OnBounce(BounceOnEnemies.OnBounceEventHandler obj)
    {
        if (obj.Target != Bounceable) return;

        obj.Collider.enabled = false;
        DefaultMachinery.AddBasicMachine(HandleActivation());
    }

    private IEnumerable<IEnumerable<Action>> HandleActivation()
    {
        Dasheable.enabled = false;
        Bounceable.enabled = false;

        if (_prefabPool.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = transform.position + EffectOffset;
        }

        yield return SwitchSprite.transform.GetAccessor()
            .LocalPosition
            .Y
            .Decrease(0.185f)
            .Over(0.5f)
            .WithAnimationCurve(ActivationAnim)
            .UsingTimer(GameTimer)
            .Build();

        OnActivation?.Invoke();
        _eventPublisher.PublishEvent(StateEvents.OnSwitch);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, OnBounce);
        this.StopObservingEvent<AbilityEvents, DashAttack.OnDashEventHandler>(AbilityEvents.OnDash, OnDash);
    }
}
