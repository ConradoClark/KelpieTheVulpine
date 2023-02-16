using System;
using Licht.Unity.Objects;
using UnityEngine;

public class UICounter : BaseGameObject
{
    public GameCounter Counter;
    public SpriteRenderer FullSprite;
    public float FullSize;
    public float EmptySize;

    protected override void OnEnable()
    {
        base.OnEnable();
        Counter.OnValueChange += AdjustBarSize;
        AdjustBarSize(Counter.Value);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Counter.OnValueChange -= AdjustBarSize;
    }

    private void AdjustBarSize(float amount)
    {
        FullSprite.size = new Vector2(CalculateSize(amount), FullSprite.size.y);
    }

    private float CalculateSize(float amount)
    {
        if (Math.Abs(amount - Counter.MinValue) < Tolerance) return EmptySize;
        return EmptySize + amount / Counter.MaxValue * (FullSize - EmptySize);
    }

    private const double Tolerance = 0.001f;
}
