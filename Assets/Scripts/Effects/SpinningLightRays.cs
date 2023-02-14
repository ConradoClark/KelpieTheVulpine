using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class SpinningLightRays : BaseGameObject
{
    public float RaysDurationInSeconds;
    public float RaysSpinVelocity;

    public Transform Mask;

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddUniqueMachine("showRays", UniqueMachine.UniqueMachineBehaviour.Cancel,
            ShowRays());
    }

    public void Show(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    private IEnumerable<IEnumerable<Action>> ShowRays()
    {
        var spinWhileYouWait = TimeYields.WaitSeconds(GameTimer, RaysDurationInSeconds, step: _ =>
        {
            transform.Rotate(0,0, RaysSpinVelocity * -0.1f
                                                   * (float)GameTimer.UpdatedTimeInMilliseconds); 
        });

        DefaultMachinery.AddBasicMachine(spinWhileYouWait);

        yield return Mask.GetAccessor()
            .UniformScale()
            .Increase(10)
            .Over(RaysDurationInSeconds * 0.3f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return TimeYields.WaitSeconds(GameTimer, RaysDurationInSeconds * 0.4f);

        yield return Mask.GetAccessor()
            .UniformScale()
            .Decrease(10)
            .Over(RaysDurationInSeconds * 0.3f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();
        gameObject.SetActive(false);
    }
}
