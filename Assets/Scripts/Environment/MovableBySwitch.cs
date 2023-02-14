using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class MovableBySwitch : BaseGameObject
{
    public SwitchObject SwitchObject;
    public Vector3 TargetPosition;
    public float DurationInSeconds;

    protected override void OnEnable()
    {
        base.OnEnable();
        SwitchObject.OnActivation += SwitchObject_OnActivation;
    }

    private void SwitchObject_OnActivation()
    {
        var move = transform.GetAccessor()
            .LocalPosition
            .ToPosition(TargetPosition)
            .Over(DurationInSeconds)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .Build();

        DefaultMachinery.AddBasicMachine(move);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SwitchObject.OnActivation -= SwitchObject_OnActivation;
    }
}
