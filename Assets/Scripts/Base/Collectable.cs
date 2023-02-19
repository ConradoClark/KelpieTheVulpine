using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class Collectable : BaseGameObject
{
    public bool CanPickupAsFox;
    public bool CanPickupAsGhost;
    public bool DeactivatesOnPickup;

    public Color GhostColor;
    public SpriteRenderer ObjectSprite;
    public SpriteRenderer OutlineSprite;
    public ScriptPrefab PickupEffect;
    public LichtPhysicsCollisionDetector CollisionDetector;

    private Player _player;
    private IEventPublisher<PickupEvents, Collectable> _eventPublisher;
    public event Action<Collectable> OnPickup;

    public bool CanPickup { get; private set; }
    public bool PickedUp { get; private set; }

    public enum PickupEvents
    {
        OnPickup
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = _player.FromScene(true);
        _eventPublisher = this.RegisterAsEventPublisher<PickupEvents, Collectable>();
        AdjustVisibility();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent(StateEvents.OnFoxForm, OnChangeForm);
        this.ObserveEvent(StateEvents.OnGhostForm, OnChangeForm);
        DefaultMachinery.AddBasicMachine(HandlePickup());
    }

    private void OnChangeForm()
    {
        if (this == null) return;
        AdjustVisibility();
    }

    private IEnumerable<IEnumerable<Action>> HandlePickup()
    {
        while (ComponentEnabled)
        {
            if (CanPickup && CollisionDetector.Triggers.Any(t => t.TriggeredHit))
            {
                PickedUp = true;
                OnPickup?.Invoke(this);
                PickupEffect.TrySpawnEffect(transform.position, out _);
                _eventPublisher.PublishEvent(PickupEvents.OnPickup, this);
                if (DeactivatesOnPickup)
                {
                    OnDisable();
                    gameObject.SetActive(false);
                }
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent(StateEvents.OnFoxForm, OnChangeForm);
        this.StopObservingEvent(StateEvents.OnGhostForm, OnChangeForm);
    }

    private void AdjustVisibility()
    {
        switch (_player.CurrentForm)
        {
            case Player.PlayerForms.Fox:
                ObjectSprite.enabled = CanPickupAsFox;
                OutlineSprite.enabled = !CanPickupAsFox;
                OutlineSprite.color = Color.white;
                CanPickup = !PickedUp && CanPickupAsFox;
                break;
            case Player.PlayerForms.Ghost:
                ObjectSprite.enabled = CanPickupAsGhost;
                OutlineSprite.enabled = !CanPickupAsGhost;
                OutlineSprite.color = GhostColor;
                CanPickup = !PickedUp && CanPickupAsGhost;
                break;
            default:
                break;
        }
    }
}
