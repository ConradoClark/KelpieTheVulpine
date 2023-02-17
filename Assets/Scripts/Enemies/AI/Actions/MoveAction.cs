using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;

public class MoveAction : BaseAIAction
{
    public float DurationInSeconds;
    public AnimationCurve XCurve;
    public AnimationCurve YCurve;

    public bool FollowPlayer;
    public bool HasExtraMovement;
    public float Speed;
    public float ExtraMovementSpeed;

    private Player _player;
    private bool _running;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance(true);
    }

    private IEnumerable<IEnumerable<Action>> Move(BaseEnemyAI source)
    {
        while (_running)
        {
            var speed = ((Vector2)((_player.CurrentForm == Player.PlayerForms.Fox
                                       ? _player.FoxForm.transform.position
                                       : _player.GhostForm.transform.position)
                                   - transform.position)).normalized * Speed;

            yield return source.Reference.GetSpeedAccessor(speed)
                .ToSpeed(Vector2.zero)
                .SetTarget(Speed)
                .Over(0.5f)
                .BreakIf(() => !_running)
                .Easing(EasingYields.EasingFunction.Linear)
                .UsingTimer(GameTimer)
                .Build();
        }
    }

    private IEnumerable<IEnumerable<Action>> ExtraMovement(BaseEnemyAI source)
    {
        while (_running)
        {
            var dir = Mathf.Sign(((_player.CurrentForm == Player.PlayerForms.Fox
                             ? _player.FoxForm.transform.position
                             : _player.GhostForm.transform.position)
                         - transform.position).x);


            var moveX = source.Reference.GetSpeedAccessor()
                .X
                .SetTarget(ExtraMovementSpeed * dir)
                .Over(DurationInSeconds)
                .BreakIf(()=>!_running)
                .WithAnimationCurve(XCurve)
                .UsingTimer(GameTimer)
                .Build();

            var moveY = source.Reference.GetSpeedAccessor()
                .Y
                .SetTarget(ExtraMovementSpeed)
                .Over(DurationInSeconds)
                .BreakIf(() => !_running)
                .WithAnimationCurve(YCurve)
                .UsingTimer(GameTimer)
                .Build();

            yield return moveX.Combine(moveY);
        }
    }

    public override IEnumerable<IEnumerable<Action>> Execute(BaseEnemyAI source, Func<bool> breakCondition)
    {
        _running = true;
        
        if (HasExtraMovement) DefaultMachinery.AddBasicMachine(ExtraMovement(source));
        if (FollowPlayer) DefaultMachinery.AddBasicMachine(Move(source));

        yield return TimeYields.WaitSeconds(GameTimer, DurationInSeconds);
        _running = false;
        yield return TimeYields.WaitOneFrameX;
    }

    public override void OnInterrupt(BaseEnemyAI source)
    {
        _running = false;
    }
}
