using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class KelpieSounds : BaseGameObject
{
    public AudioSource AudioSource;
    public AudioSource PickupAudioSource;
    public AudioSource EnemyAudioSource;
    public AudioClip JumpingSound;
    public AudioClip SpringBounceSound;
    public AudioClip CoinSound;
    public AudioClip CheckpointSound;
    public AudioClip SwitchSound;

    private Player _player;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = _player.FromScene();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents,
            LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart,
            OnJump);
        this.ObserveEvent(Coin.CoinEvents.OnPickup, OnCoinPickup);

        this.ObserveEvent<StateEvents, Killable>(StateEvents.OnDeath, OnEnemyDeath);
        this.ObserveEvent<AbilityEvents>(AbilityEvents.OnSpringBounce, OnSpringBounce);
        this.ObserveEvent<Checkpoint.CheckpointEvents, Checkpoint>(Checkpoint.CheckpointEvents.OnCheckPoint,
            OnCheckpoint);

        this.ObserveEvent<StateEvents, FollowingInventory>(StateEvents.OnUse, OnUseItem);
        this.ObserveEvent<Collectable.PickupEvents, Collectable>(Collectable.PickupEvents.OnPickup, OnCollect);

        this.ObserveEvent(StateEvents.OnSwitch, OnSwitch);
    }

    private void OnSwitch()
    {
        PickupAudioSource.pitch = Random.Range(0.9f, 1.1f);
        PickupAudioSource.PlayOneShot(SwitchSound);
    }

    private void OnCollect(Collectable obj)
    {
        if (obj.SoundOnCollect != null)
        {
            PickupAudioSource.pitch = Random.Range(0.9f, 1.1f);
            PickupAudioSource.PlayOneShot(obj.SoundOnCollect);
        }
    }

    private void OnUseItem(FollowingInventory obj)
    {
        if (obj.SoundOnUse != null)
        {
            PickupAudioSource.pitch = 1f;
            PickupAudioSource.PlayOneShot(obj.SoundOnUse);
        }
            
    }

    private void OnCheckpoint(Checkpoint obj)
    {
        if (obj.Default) return;

        AudioSource.PlayOneShot(CheckpointSound);
    }

    private void OnSpringBounce()
    {
        AudioSource.PlayOneShot(SpringBounceSound);
    }

    private void OnEnemyDeath(Killable obj)
    {
        if (obj.DeathSound != null)
        {
            EnemyAudioSource.PlayOneShot(obj.DeathSound);
        }

    }

    private void OnCoinPickup()
    {
        PickupAudioSource.pitch = Random.Range(0.9f, 1.1f);
        PickupAudioSource.PlayOneShot(CoinSound, 0.8f);
    }

    private void OnJump(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
    {
        if (obj.Source.IsForcedJump) return;
        AudioSource.PlayOneShot(JumpingSound);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents,
            LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart,
            OnJump);

        this.StopObservingEvent(Coin.CoinEvents.OnPickup, OnCoinPickup);
        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnDeath, OnEnemyDeath);
        this.StopObservingEvent<AbilityEvents>(AbilityEvents.OnSpringBounce, OnSpringBounce);
        this.StopObservingEvent<Checkpoint.CheckpointEvents, Checkpoint>(Checkpoint.CheckpointEvents.OnCheckPoint,
            OnCheckpoint);

        this.StopObservingEvent<StateEvents, FollowingInventory>(StateEvents.OnUse, OnUseItem);
        this.StopObservingEvent<Collectable.PickupEvents, Collectable>(Collectable.PickupEvents.OnPickup, OnCollect);

        this.StopObservingEvent(StateEvents.OnSwitch, OnSwitch);
    }
}
