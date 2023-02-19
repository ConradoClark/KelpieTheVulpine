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
    }
}
