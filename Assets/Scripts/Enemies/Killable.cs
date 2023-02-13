using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;

public class Killable : EffectPoolable
{
    public GameObject Actor;
    public bool SceneObject;
    public bool Killed;

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
        this.ObserveEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);    
    }

    private void OnDeath(Killable obj)
    {
        if (obj != this || Killed) return;

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
        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
    }
}
