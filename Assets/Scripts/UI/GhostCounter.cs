using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using UnityEngine;

public class GhostCounter : GameCounter
{
    public override float Value => _value;
    private float _value;

    [SerializeField] private float _minValue;
    public override float MinValue => _minValue;

    [SerializeField] private float _maxValue;
    public override float MaxValue => _maxValue;
    public override event Action<float> OnValueChange;

    public TintFlash RecoverFlash;

    protected override void OnAwake()
    {
        base.OnAwake();
        _value = MaxValue;
    }

    public void Drain(float amount)
    {
        _value = Mathf.Max(_minValue, _value - amount);
        OnValueChange?.Invoke(_value);
    }

    public void Recover(float amount)
    {
        DefaultMachinery.AddBasicMachine(RecoverFlash.Flash());
        _value = Mathf.Min(_maxValue, _value + amount);
        OnValueChange?.Invoke(_value);
    }
}
