using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using TMPro;

public class DeathsCounter : BaseGameObject
{
    public TMP_Text Counter;
    private int _number;

    private KillableFox _playerKillable;

    protected override void OnAwake()
    {
        base.OnAwake();
        _playerKillable = _playerKillable.FromScene();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
    }

    private void OnDeath(Killable obj)
    {
        if (_playerKillable == obj) _number += 1;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
    }

    public void ShowText()
    {
        Counter.text = _number.ToString().PadLeft(2, '0');
    }
}
