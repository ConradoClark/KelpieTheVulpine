using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DarkWorldController : BaseGameObject
{
    [field:SerializeField]
    public Volume DarkWorldVolume { get; private set; }
    public bool IsInDarkWorld { get; private set; }

    private Vignette _vignette;
    private Player _player;
    private Camera _gameCamera;

    protected override void OnAwake()
    {
        base.OnAwake();
        DarkWorldVolume.profile.TryGet(out _vignette);
        _player = SceneObject<Player>.Instance();
        _gameCamera = Camera.main;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public IEnumerable<IEnumerable<Action>> GoToDarkWorld()
    {
        IsInDarkWorld = true;

        var fade = new LerpBuilder(f => DarkWorldVolume.weight = f,
                () => DarkWorldVolume.weight)
            .SetTarget(1f)
            .Over(2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        foreach (var action in fade)
        {
            action();
            SetVignettePositionToPlayer();
            yield return TimeYields.WaitOneFrameX;
        }

        while (IsInDarkWorld)
        {
            SetVignettePositionToPlayer();
            yield return TimeYields.WaitOneFrameX;
        }
    }

    public IEnumerable<IEnumerable<Action>> ExitFromDarkWorld()
    {
        IsInDarkWorld = false;

        yield return new LerpBuilder(f => DarkWorldVolume.weight = f,
                () => DarkWorldVolume.weight)
            .SetTarget(0f)
            .Over(2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();
    }

    private void SetVignettePositionToPlayer()
    {
        var center = (Vector2)_gameCamera.WorldToViewportPoint(_player.IsFox
            ? _player.FoxForm.transform.position
            : _player.GhostForm.transform.position);

        _vignette.center.SetValue(new Vector2Parameter(center, true));
    }
}
