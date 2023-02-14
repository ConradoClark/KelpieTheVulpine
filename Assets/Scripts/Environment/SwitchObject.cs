using System;
using Licht.Unity.Objects;      
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
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

    private PrefabPool _prefabPool;

    protected override void OnAwake()
    {
        base.OnAwake();
        _prefabPool = SceneObject<EffectsManager>.Instance().GetEffect(InteractionEffect);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, OnBounce);
        this.ObserveEvent<AbilityEvents, DashAttack.OnDashEventHandler>(AbilityEvents.OnDash, OnDash);
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
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, OnBounce);
        this.StopObservingEvent<AbilityEvents, DashAttack.OnDashEventHandler>(AbilityEvents.OnDash, OnDash);
    }
}