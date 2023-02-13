using Licht.Impl.Events;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class KillableByBouncing : BaseGameObject
{
    public Collider2D Collider;

    private Killable _killable;
    private IEventPublisher<StateEvents, Killable> _eventPublisher;

    protected override void OnAwake()
    {
        base.OnAwake();
        _killable = GetComponent<Killable>() ?? GetComponentInParent<Killable>(true);
        _eventPublisher = this.RegisterAsEventPublisher<StateEvents, Killable>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, BouncedOn);
    }

    private void BouncedOn(BounceOnEnemies.OnBounceEventHandler obj)
    {
        if (obj.Collider != Collider) return;
        _eventPublisher.PublishEvent(StateEvents.OnDeath, _killable);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<AbilityEvents, BounceOnEnemies.OnBounceEventHandler>(AbilityEvents.OnBounce, BouncedOn);
    }
}
