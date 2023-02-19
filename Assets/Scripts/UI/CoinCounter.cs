using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;
using TMPro;

public class CoinCounter : BaseGameObject
{
    public TMP_Text CounterText;

    private int _totalCoins;
    private int _collectedCoins;

    protected override void OnAwake()
    {
        base.OnAwake();
        CountCoins();
        SetText();
    }

    private void CountCoins()
    {
        _totalCoins = FindObjectsOfType<Coin>().Length;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent(Coin.CoinEvents.OnPickup, OnEvent);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent(Coin.CoinEvents.OnPickup, OnEvent);
    }

    private void OnEvent()
    {
        _collectedCoins++;
        SetText();
    }

    private void SetText()
    {
        CounterText.text = $"{_collectedCoins.ToString().PadLeft(2, '0')} / {_totalCoins.ToString().PadLeft(2, '0')}";

    }
}
