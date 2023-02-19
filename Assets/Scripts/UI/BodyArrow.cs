using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class BodyArrow : BaseGameObject
{
    public SpriteRenderer Sprite;
    private Player _player;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = _player.FromScene();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent(StateEvents.OnFoxForm, OnChangeForm);
        this.ObserveEvent(StateEvents.OnGhostForm, OnChangeForm);
    }

    private void OnChangeForm()
    {
        if (Sprite == null) return;

        Sprite.enabled = _player.CurrentForm == Player.PlayerForms.Ghost;
    }

    private void LateUpdate()
    {
        if (_player.CurrentForm != Player.PlayerForms.Ghost) return;

        var rotation = Quaternion.LookRotation(_player.GhostForm.transform.position
                                               - transform.position,
            Vector3.forward);

        var angle = ((Vector2)(_player.FoxForm.transform.position - _player.GhostForm.transform.position)).normalized;
        transform.SetPositionAndRotation(_player.GhostForm.transform.position + (Vector3)angle * 0.5f,
            new Quaternion(0, 0, rotation.z, rotation.w));
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent(StateEvents.OnFoxForm, OnChangeForm);
        this.StopObservingEvent(StateEvents.OnGhostForm, OnChangeForm);
    }
}
