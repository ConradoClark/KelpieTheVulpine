using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Physics;
using UnityEngine;

public class PhysicalEntity : MonoBehaviour
{
    public LichtPhysicsObject PhysicsObject;
    public Killable Killable;

    private void Awake()
    {
        PhysicsObject.AddCustomObject(this);
    }
}
