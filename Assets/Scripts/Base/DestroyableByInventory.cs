using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class DestroyableByInventory : BaseGameObject
{
    public string Identifier;
    public float TriggerDistance;
    public ScriptPrefab DestroyEffect;
    private Player _player;
    protected override void OnAwake()
    {
        base.OnAwake();
        _player = _player.FromScene();
    }

    protected override void OnEnable()
    { 
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(Handle());
    }

    private IEnumerable<IEnumerable<Action>> Handle()
    {
        while (ComponentEnabled)
        {
            if (_player.TryGetFromInventory(Identifier, out var item) && 
                Vector2.Distance(transform.position, _player.CurrentFormPosition) < TriggerDistance)
            {
                item.Triggered = true;

                yield return item.transform.GetAccessor()
                    .Position
                    .ToPosition(transform.position)
                    .Over(0.5f)
                    .UsingTimer(GameTimer)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                    .Build();

                DestroyEffect.TrySpawnEffect(transform.position, out _);
                _player.RemoveFromInventory(item);
                item.EndEffect();
                gameObject.SetActive(false);

            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
