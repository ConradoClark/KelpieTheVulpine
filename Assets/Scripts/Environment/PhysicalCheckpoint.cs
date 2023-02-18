using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class PhysicalCheckpoint : Checkpoint
{
    public LichtPhysicsCollisionDetector CollisionDetector;

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleCheckpoint());
        this.ObserveEvent<CheckpointEvents,Checkpoint>(CheckpointEvents.OnCheckPoint, OnCheckpoint);
    }

    private void OnCheckpoint(Checkpoint obj)
    {
        if (obj == this) return;

        Activated = false;
        EventPublisher.PublishEvent(CheckpointEvents.OnCheckpointDeactivated, this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<CheckpointEvents, Checkpoint>(CheckpointEvents.OnCheckPoint, OnCheckpoint);
    }

    protected override IEnumerable<IEnumerable<Action>> HandleCheckpoint()
    {
        while (ComponentEnabled)
        {
            if (CollisionDetector.Triggers.Any(t => t.TriggeredHit) 
                && !Activated)
            {
                Activated = true;
                UpdateCheckpointPosition();
                EventPublisher.PublishEvent(CheckpointEvents.OnCheckPoint, this);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
