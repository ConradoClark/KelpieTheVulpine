using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class Float : BaseGameObject
{
    public float Height;
    public float FrequencyInSeconds;

    private float _initialY;
    protected override void OnAwake()
    {
        base.OnAwake();
        _initialY = transform.localPosition.y;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleFloat());
    }

    private IEnumerable<IEnumerable<Action>> HandleFloat()
    {
        while (ComponentEnabled)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, _initialY, transform.localPosition.z);
            yield return transform.GetAccessor()
                .LocalPosition
                .Y
                .Increase(Height)
                .Over(FrequencyInSeconds * 0.5f)
                .BreakIf(()=>!ComponentEnabled)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return transform.GetAccessor()
                .LocalPosition
                .Y
                .Decrease(Height)
                .Over(FrequencyInSeconds * 0.5f)
                .BreakIf(() => !ComponentEnabled)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();
        }
    }
}
