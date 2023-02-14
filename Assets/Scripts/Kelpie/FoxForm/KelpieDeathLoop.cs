using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class KelpieDeathLoop : BaseGameObject
{
    private Player _player;
    public SpriteRenderer[] EffectSprites;
    public Color SpriteColor;

    public void Show()
    {
        _player ??= SceneObject<Player>.Instance();
        transform.position = _player.FoxForm.transform.position;
        gameObject.SetActive(true);
        foreach (var effect in EffectSprites)
        {
            effect.color = new Color(0, 0, 0, 0);
            var appear = effect.GetAccessor().Color
                .ToColor(SpriteColor)
                .Over(2)
                .Easing(EasingYields.EasingFunction.CubicEaseIn)
                .UsingTimer(GameTimer)
                .Build();

            DefaultMachinery.AddBasicMachine(appear);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
