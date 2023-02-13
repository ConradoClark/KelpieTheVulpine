using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Physics;
using UnityEngine;

public class GhostFormPlayer : MonoBehaviour
{
    public LichtPhysicsObject PhysicsObject;
    public LichtTopDownMoveController MoveController;
    public GhostFollowMouse GhostMouseFollower;
}
