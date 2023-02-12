using System.Collections;
using System.Collections.Generic;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class FoxFormPlayer : MonoBehaviour
{
    public LichtPlatformerMoveController MoveController;
    public LichtPlatformerJumpController JumpController;
    public Basic2DBoxCast GroundCollider;
    public LichtPhysicsObject PhysicsObject;
}
