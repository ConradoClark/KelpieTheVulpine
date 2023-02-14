using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Effects;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;

public class PoofKillable : Killable
{
    public ScriptPrefab PoofEffect;
    public Animator Animator;
    public TintFlash TintFlash;

    private PrefabPool _poofPool;
    private Collider2D[] _colliders;
    private FoxFormPlayer _player;

    protected override void OnAwake()
    {
        base.OnAwake();
        _poofPool = SceneObject<EffectsManager>.Instance().GetEffect(PoofEffect);
        _colliders = GetComponents<Collider2D>();
        _player = SceneObject<Player>.Instance().FoxForm;
    }

    private IEnumerable<IEnumerable<Action>> SlowDown()
    {
        yield return new LerpBuilder(f => GameTimer.Multiplier = f,
                () => (float)GameTimer.Multiplier)
            .SetTarget(0.01f)
            .Over(0.1f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(UITimer)
            .Build();

        yield return TimeYields.WaitMilliseconds(UITimer, 10);

        yield return new LerpBuilder(f => GameTimer.Multiplier = f,
                () => (float)GameTimer.Multiplier)
            .SetTarget(1f)
            .Over(0.1f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(UITimer)
            .Build();
    }

    protected override IEnumerable<IEnumerable<Action>> OnDeathEffect()
    {
        var deactivated = new List<Collider2D>();
        foreach (var col in _colliders)
        {
            if (!col.enabled) continue;
            col.enabled = false;
            deactivated.Add(col);
        }

        DefaultMachinery.AddBasicMachine(SlowDown());

        if (_poofPool.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = transform.position;
        }

        yield return TintFlash.Flash(true).AsCoroutine();

        foreach (var col in deactivated)
        {
            col.enabled = true;
        }
        yield return base.OnDeathEffect().AsCoroutine();
    }
}
