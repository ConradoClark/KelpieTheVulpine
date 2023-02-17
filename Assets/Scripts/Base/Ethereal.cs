using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Ethereal : BaseGameObject
{
    private LichtPhysicsObject _physicsObject;
    private void Awake()
    {
        _physicsObject = GetComponent<LichtPhysicsObject>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _physicsObject.AddCustomObject(this);
    }
}