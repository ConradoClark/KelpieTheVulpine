using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class BounceOnEnemies : BaseGameObject
{
    public LayerMask LayerToCheck;
    public LichtPlatformerJumpController.CustomJumpParams BounceParams;
    public float MinJumpDelay;
    private Player _player;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        BounceParams.MinJumpDelay = MinJumpDelay;
        DefaultMachinery.AddBasicMachine(HandleBounce());
    }

    private IEnumerable<IEnumerable<Action>> HandleBounce()
    {
        while (ComponentEnabled)
        {
            var conditions = _player.IsFox
                             && !_player.FoxForm.JumpController.IsJumping
                             && _player.FoxForm.PhysicsObject.GetPhysicsTrigger(_player.FoxForm.JumpController
                                 .GroundedTrigger)
                             && _player.FoxForm.GroundCollider.Triggers.Any(t =>
                                 LayerToCheck.Contains(t.Collider.gameObject.layer));
            if (conditions)
            {
                yield return _player.FoxForm.JumpController.ExecuteJump(customParams: BounceParams).AsCoroutine();
            }
            else yield return TimeYields.WaitOneFrameX;
        }
    }
}
