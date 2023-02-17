using Licht.Impl.Events;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class KillableByDashing : BaseGameObject
{
    public Collider2D Collider;

    public bool KnockBack;

    private KnockBackable _knockBackable;
    private Killable _killable;
    private IEventPublisher<StateEvents, Killable> _eventPublisher;
    private IEventPublisher<StateEvents, KnockBack> _knockBackPublisher;

    protected override void OnAwake()
    {
        base.OnAwake();
        _killable = GetComponent<Killable>() ?? GetComponentInParent<Killable>(true);
        _knockBackable = GetComponent<KnockBackable>() ?? GetComponentInParent<KnockBackable>(true);
        _eventPublisher = this.RegisterAsEventPublisher<StateEvents, Killable>();
        _knockBackPublisher = this.RegisterAsEventPublisher<StateEvents, KnockBack>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<AbilityEvents, DashAttack.OnDashEventHandler>(AbilityEvents.OnDash, DashedOn);
    }

    private void DashedOn(DashAttack.OnDashEventHandler obj)
    {
        if (obj.Collider != Collider) return;
        _eventPublisher.PublishEvent(StateEvents.OnDeathAttempt, _killable);

        if (KnockBack && _knockBackable!=null)
        {
            _knockBackPublisher.PublishEvent(StateEvents.OnKnockBack, new KnockBack
            {
                Target = _knockBackable,
                Speed = obj.Speed
            });
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<AbilityEvents, DashAttack.OnDashEventHandler>(AbilityEvents.OnDash, DashedOn);
    }
}
