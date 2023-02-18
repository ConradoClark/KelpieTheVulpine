using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class Checkpoint : BaseGameObject
{
    public static Vector3 CurrentCheckpointPosition;
    public Vector3 Offset;
    protected IEventPublisher<CheckpointEvents, Checkpoint> EventPublisher;
    public bool Activated { get; protected set; }
    public enum CheckpointEvents
    {
        OnCheckPoint,
        OnCheckpointDeactivated
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        EventPublisher = this.RegisterAsEventPublisher<CheckpointEvents, Checkpoint>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleCheckpoint());
    }

    protected virtual IEnumerable<IEnumerable<Action>> HandleCheckpoint()
    {
        Activated = true;
        UpdateCheckpointPosition();
        yield break;
    }

    protected void UpdateCheckpointPosition()
    {
        CurrentCheckpointPosition = transform.position + Offset;
    }
}
