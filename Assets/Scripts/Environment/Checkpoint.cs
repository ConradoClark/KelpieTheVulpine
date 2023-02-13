using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class Checkpoint : BaseGameObject
{
    public static Vector3 CurrentCheckpointPosition;
    public Vector3 Offset;

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleCheckpoint());
    }

    protected virtual IEnumerable<IEnumerable<Action>> HandleCheckpoint()
    {
        CurrentCheckpointPosition = transform.position + Offset;
        yield break;
    }
}
