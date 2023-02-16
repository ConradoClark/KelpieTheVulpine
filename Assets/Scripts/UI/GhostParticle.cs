using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostParticle : EffectPoolable
{
    public float Speed;
    public float RecoverAmount;
    public Vector3 TargetOffset;
    private GhostCounter _ghostCounter;

    protected override void OnAwake()
    {
        base.OnAwake();
        _ghostCounter = SceneObject<GhostCounter>.Instance(true);
    }

    public override void OnActivation()
    {
        DefaultMachinery.AddBasicMachine(Travel());
    }

    private IEnumerable<IEnumerable<Action>> Travel()
    {
        yield return TimeYields.WaitOneFrameX;

        var dir = Random.insideUnitCircle;
        var target = dir * Speed;
         yield return transform.GetAccessor()
            .Position
            .ToPosition( transform.position + (Vector3) target)
            .Over(0.5f + Random.value * 0.3f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(UITimer)
            .Build();

        var homingX = transform.GetAccessor()
            .Position
            .X
            .SetTarget(_ghostCounter.transform.position.x + TargetOffset.x)
            .Over(0.6f + Random.value * 0.5f)
            .FromUpdatedValues()
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(UITimer)
            .Build();

        var homingY = transform.GetAccessor()
            .Position
            .Y
            .SetTarget(_ghostCounter.transform.position.y + TargetOffset.y)
            .Over(0.4f + Random.value * 0.5f)
            .FromUpdatedValues()
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(UITimer)
            .Build();

        yield return homingX.Combine(homingY);

        _ghostCounter.Recover(RecoverAmount);

        EndEffect();
    }
}
