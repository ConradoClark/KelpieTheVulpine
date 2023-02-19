using Licht.Impl.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;

public class EnemiesKilledCounter : BaseGameObject
{
    public LayerMask Layers;
    public TMP_Text Counter;
    private int _number;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<StateEvents,Killable>(StateEvents.OnDeath, OnDeath);
    }

    private void OnDeath(Killable obj)
    {
        if (Layers.Contains(obj.gameObject.layer))
        {
            _number += 1;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
    }

    public void ShowText()
    {
        Counter.text = _number.ToString().PadLeft(3, '0');
    }
}
