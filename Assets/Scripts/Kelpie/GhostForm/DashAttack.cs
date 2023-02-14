using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Effects;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAttack : BaseGameObject
{
    public float DashSpeed;
    public InputActionReference DashInput;
    public InputActionReference MouseClick;
    public TintFlash DashFlash;
    public GhostFollowMouse MouseFollow;
    public ScriptPrefab TrailEffect;
    public SpriteRenderer DashEffect;

    private GhostFormPlayer _player;
    private PrefabPool _trailPool;

    public bool IsDashing { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance().GhostForm;
        _trailPool = SceneObject<EffectsManager>.Instance().GetEffect(TrailEffect);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleDash());
    }

    private IEnumerable<IEnumerable<Action>> HandleTrail()
    {
        while (IsDashing)
        {
            if (_trailPool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position;
            }

            yield return TimeYields.WaitMilliseconds(GameTimer, 25);
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleDash()
    {
        while (ComponentEnabled)
        {
            if (DashInput.action.WasPerformedThisFrame())
            {
                _player.MoveController.BlockMovement(this);
                MouseFollow.BlockMovement(this);

                IsDashing = true;
                DashEffect.enabled = true;

                DefaultMachinery.AddBasicMachine(HandleTrail());

                var latestDirection = MouseClick.action.WasPerformedThisFrame() ?
                    MouseFollow.GetDirectionFromMouse()
                    : _player.MoveController.LatestDirection;

                var move = _player.PhysicsObject.GetSpeedAccessor(latestDirection * DashSpeed)
                    .ToSpeed(Vector2.zero)
                    .Over(0.5f)
                    .BreakIf(()=>!ComponentEnabled && IsDashing)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(GameTimer)
                    .Build();

                yield return DashFlash.Flash().AsCoroutine().Combine(move);
                _player.MoveController.UnblockMovement(this);
                MouseFollow.UnblockMovement(this);
                
                DashEffect.enabled = false;
                IsDashing = false;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
