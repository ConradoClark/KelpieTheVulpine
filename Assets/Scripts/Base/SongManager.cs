using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class SongManager : BaseGameObject
{
    public AudioSource AudioSource;
    public AudioClip Song;
    public AudioClip GhostSong;

    private float _currentTime;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent(StateEvents.OnFoxForm, OnFox);
        this.ObserveEvent(StateEvents.OnGhostForm, OnGhost);
    }

    private void OnFox()
    {
        AudioSource.time = _currentTime;
        AudioSource.clip = Song;
        AudioSource.Play();
    }

    private void OnGhost()
    {
        _currentTime = AudioSource.time;
        AudioSource.time = 0f;
        AudioSource.clip = GhostSong;
        AudioSource.Play();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent(StateEvents.OnFoxForm, OnFox);
        this.StopObservingEvent(StateEvents.OnGhostForm, OnGhost);
    }
}