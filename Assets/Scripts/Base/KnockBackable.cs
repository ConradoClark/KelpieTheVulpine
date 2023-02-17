using System;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class KnockBackable : BaseGameObject
{
    private LichtPhysicsObject _physicsObject;

    protected override void OnAwake()
    {
        base.OnAwake();
        _physicsObject = GetComponent<LichtPhysicsObject>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<StateEvents,KnockBack>(StateEvents.OnKnockBack, OnKnockBack);
    }

    private void OnKnockBack(KnockBack obj)
    {
        if (obj.Target != this) return;

        var move = _physicsObject.GetSpeedAccessor(obj.Speed)
            .ToSpeed(Vector2.zero)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        DefaultMachinery.AddBasicMachine(move);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<StateEvents, KnockBack>(StateEvents.OnKnockBack, OnKnockBack);
    }
}
