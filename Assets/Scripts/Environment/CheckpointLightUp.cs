using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class CheckpointLightUp : BaseGameObject
{
    public SpriteRenderer CheckpointPoleSpriteRenderer;
    public Sprite PoleActive;
    public Sprite PoleInactive;
    public TintFlash ActivationFlash;

    public SpriteRenderer GhostSparkSpriteRenderer;
    private Checkpoint _checkPoint;

    protected override void OnAwake()
    {
        base.OnAwake();
        _checkPoint = GetComponent<Checkpoint>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<Checkpoint.CheckpointEvents, Checkpoint>(Checkpoint.CheckpointEvents.OnCheckPoint,
            OnCheckpoint);
    }

    private void OnCheckpoint(Checkpoint obj)
    {
        if (obj == _checkPoint)
        {
            Activate();
        }

        else Deactivate();
    }

    private void Activate()
    {
        CheckpointPoleSpriteRenderer.sprite = PoleActive;
        GhostSparkSpriteRenderer.enabled = true;
        DefaultMachinery.AddBasicMachine(ActivationFlash.Flash());
    }

    private void Deactivate()
    {
        CheckpointPoleSpriteRenderer.sprite = PoleInactive;
        GhostSparkSpriteRenderer.enabled = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<Checkpoint.CheckpointEvents, Checkpoint>(Checkpoint.CheckpointEvents.OnCheckPoint,
            OnCheckpoint);
    }
}
