using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class TintFlash : BaseGameObject
{
    public SpriteRenderer SpriteRenderer;
    public Color FlashColor;
    public float FlashSpeed;
    public int Repeat;

    public IEnumerable<IEnumerable<Action>> Flash(bool disappear=false)
    {
        var original = SpriteRenderer.material.GetColor("_Tint");
        for (var i = 0; i < Repeat + 1; i++)
        {
            yield return SpriteRenderer.GetAccessor()
                .Material("_Tint")
                .AsColor()
                .ToColor(FlashColor)
                .Over(FlashSpeed * 0.75f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(GameTimer)
                .Build();

            var back = SpriteRenderer.GetAccessor()
                .Material("_Tint")
                .AsColor()
                .ToColor(original)
                .Over(FlashSpeed * 0.25f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(GameTimer)
                .Build();

            if (disappear && i == Repeat)
            {
                yield return back.Combine(SpriteRenderer.GetAccessor()
                    .Color
                    .A
                    .SetTarget(0)
                    .Over(FlashSpeed * 0.25f)
                    .UsingTimer(GameTimer)
                    .Easing(EasingYields.EasingFunction.CubicEaseIn)
                    .Build());
            }
            else
            {
                yield return back;
            }
        }
    }
}
