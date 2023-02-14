using Licht.Impl.Events;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using UnityEngine;

public class FoxFormAnimator : BaseGameObject
{
    public Animator Animator;
    public bool FreezeDirection;
    public SpriteRenderer SpriteRenderer;
    public Killable Killable;

    private FoxFormPlayer _player;
    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance(true).FoxForm;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents,
            LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnStartMoving);

        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents,
            LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStopMoving, OnStopMoving);

        this.ObserveEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
        this.ObserveEvent<StateEvents, Killable>(StateEvents.OnRevive, OnRevive);
    }

    private void OnRevive(Killable obj)
    {
        if (obj != Killable) return;

        Animator.SetBool("Dead", false);
    }

    private void OnDeath(Killable obj)
    {
        if (obj != Killable) return;

        Animator.SetBool("Dead", true);
    }

    private void OnStopMoving(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source != _player.MoveController) return;

        Animator.SetBool("IsWalking", false);
    }

    private void OnStartMoving(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source != _player.MoveController) return;

        Animator.SetBool("IsWalking", true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents,
            LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnStartMoving);
        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents,
            LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStopMoving, OnStopMoving);

        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnRevive, OnRevive);
    }

    private void Update()
    {
        Animator.speed = (float) GameTimer.Multiplier;
        if (FreezeDirection) return;

        SpriteRenderer.flipX = _player.MoveController.LatestDirection < 0;
    }
}
