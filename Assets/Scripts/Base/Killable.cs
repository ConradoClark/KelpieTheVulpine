using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Pooling;
using UnityEngine;

public class Killable : EffectPoolable
{
    public GameObject Actor;
    public bool SceneObject;
    public bool Killed;
    private IEventPublisher<StateEvents, Killable> _killEvent;

    protected override void OnAwake()
    {
        base.OnAwake();
        _killEvent = this.RegisterAsEventPublisher<StateEvents, Killable>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (SceneObject)
        {
            OnActivation();
        }
    }

    public override void OnActivation()
    {
        Killed = false;
        this.ObserveEvent<StateEvents, Killable>(StateEvents.OnDeathAttempt, OnDeathAttempt);    
    }

    protected virtual bool CanBeKilled()
    {
        return true;
    }

    private void OnDeathAttempt(Killable obj)
    {
        if (obj != this || Killed || !CanBeKilled()) return;

        _killEvent.PublishEvent(StateEvents.OnDeath, this);
        Killed = true;
        DefaultMachinery.AddBasicMachine(OnDeathEffect());
    }

    protected virtual IEnumerable<IEnumerable<Action>> OnDeathEffect()
    {
        Actor.gameObject.SetActive(false);
        yield break;
    }

    protected override void OnDisable()
    {
        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeathAttempt);
    }
}
