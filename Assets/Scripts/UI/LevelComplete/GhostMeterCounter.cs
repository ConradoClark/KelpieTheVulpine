using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;

public class GhostMeterCounter : BaseGameObject
{
    public TMP_Text Counter;
    private float _number;

    private GhostCounter _ghostCounter;

    protected override void OnAwake()
    {
        base.OnAwake();
        _ghostCounter = _ghostCounter.FromScene();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _ghostCounter.OnValueChange += _ghostCounter_OnValueChange;
    }

    private void _ghostCounter_OnValueChange(float obj)
    {
        _number += _ghostCounter.Value - obj;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _ghostCounter.OnValueChange -= _ghostCounter_OnValueChange;
    }

    public void ShowText()
    {
        Counter.text = Mathf.RoundToInt(_number).ToString().PadLeft(3, '0');
    }
}
