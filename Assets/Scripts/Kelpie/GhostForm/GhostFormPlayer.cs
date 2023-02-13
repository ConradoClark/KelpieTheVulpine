using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.Forces;
using UnityEngine;

public class GhostFormPlayer : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public LichtTopDownMoveController MoveController;
    public GhostFollowMouse GhostMouseFollower;
    private Gravity _gravity;

    protected override void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(DisableGravityForGhost());
    }
    private IEnumerable<Action> DisableGravityForGhost()
    {
        yield return TimeYields.WaitOneFrame;
        _gravity = SceneObject<Gravity>.Instance();
        _gravity.BlockForceFor(this, PhysicsObject);
    }
}
