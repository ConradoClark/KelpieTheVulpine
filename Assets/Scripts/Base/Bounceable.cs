using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Physics;
using UnityEngine;

public class Bounceable : MonoBehaviour
{
    private LichtPhysicsObject _physicsObject;
    private void Awake()
    {
        _physicsObject = GetComponent<LichtPhysicsObject>();
    }
    private void OnEnable()
    {
        _physicsObject.AddCustomObject(this);
    }
}
