using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameOverVolumeController : BaseGameObject
{
    [field:SerializeField]
    public Volume GameOverVolume { get; private set; }
    public bool IsGameOver { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        GameOverVolume.weight = 0f;
    }

    public IEnumerable<IEnumerable<Action>> GameOver()
    {
        IsGameOver = true;

        yield return new LerpBuilder(f => GameOverVolume.weight = f,
                () => GameOverVolume.weight)
            .SetTarget(1f)
            .Over(2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();
    }
}
