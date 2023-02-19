using System;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class MovableBySwitch : BaseGameObject
{
    public SwitchObject SwitchObject;
    public Vector3 TargetPosition;
    public float DurationInSeconds;
    private Vector3 _originalPosition;

    public event Action<Vector2> OnMove;
    private Vector2 _currentMovement;

    protected override void OnAwake()
    {
        base.OnAwake();
        _originalPosition = transform.localPosition;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SwitchObject.OnActivation += SwitchObject_OnActivation;
        SwitchObject.OnReset += SwitchObject_OnReset;
    }

    private void NotifyMovement(float f, Vector3 sourcePos, Vector3 targetPos, float durationInSeconds)
    {
        var result =  Vector2.Lerp(sourcePos, targetPos, f) - (Vector2) transform.localPosition;
        _currentMovement = result;
        OnMove?.Invoke(result);
    }

    private void SwitchObject_OnActivation()
    {
        var localPos = transform.localPosition;
        var move = transform.GetAccessor()
            .LocalPosition
            .ToPosition(TargetPosition)
            .Over(DurationInSeconds)
            .UsingTimer(GameTimer)
            .OnEachStep(f =>
            {
                NotifyMovement(f, localPos, TargetPosition, DurationInSeconds);
            })
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .Build();

        DefaultMachinery.AddBasicMachine(move);
    }

    private void SwitchObject_OnReset()
    {
        var localPos = transform.localPosition;
        var move = transform.GetAccessor()
            .LocalPosition
            .ToPosition(_originalPosition)
            .Over(DurationInSeconds)
            .UsingTimer(GameTimer)
            .OnEachStep(f =>
            {
                NotifyMovement(f, localPos, _originalPosition, DurationInSeconds);
            })
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
