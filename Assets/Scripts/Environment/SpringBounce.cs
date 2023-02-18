using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class SpringBounce : BaseGameObject
{
    private Animator _animator;
    private Bounceable _bounceable;
    public ScriptPrefab BounceEffect;


    protected override void OnAwake()
    {
        base.OnAwake();
        _animator = GetComponent<Animator>();
        _bounceable = GetComponent<Bounceable>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, OnEvent);
    }

    private void OnEvent(BounceOnEnemies.OnBounceEventHandler obj)
    {
        if (obj.Target != _bounceable) return;

        BounceEffect.TrySpawnEffect(transform.position, out _);
        _animator.SetTrigger("Bounce");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, OnEvent);

    }
}
