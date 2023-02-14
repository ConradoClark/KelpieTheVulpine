using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class KillsPhysicalOnTouch : BaseGameObject
{
    public LichtPhysicsCollisionDetector HitBox;
    public Killable Killable;
    private LichtPhysics _physics;
    private IEventPublisher<StateEvents, Killable> _killEvent;
    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
        _killEvent = this.RegisterAsEventPublisher<StateEvents, Killable>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleTouch());
    }

    private IEnumerable<IEnumerable<Action>> HandleTouch()
    {
        while (ComponentEnabled)
        {
            PhysicalEntity physicalEntity = null;
            var hit = HitBox.Triggers.FirstOrDefault(t => t.TriggeredHit
                                                                     && _physics.TryGetPhysicsObjectByCollider(t.Collider,
                                                                         out var physicsObject)
                                                                     && physicsObject.TryGetCustomObject(out physicalEntity));

            if ((Killable == null || !Killable.Killed) && 
                hit.TriggeredHit && physicalEntity != null && physicalEntity.Killable != null)
            {
                _killEvent.PublishEvent(StateEvents.OnDeathAttempt, physicalEntity.Killable);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
