using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

public class LevelCompleteOverlay : BaseGameObject
{
    public SpriteRenderer SpriteRenderer;
    public GameObject Menu;
    public DeathsCounter DeathsCounter;
    public EnemiesKilledCounter EnemiesKilledCounter;
    public GhostMeterCounter GhostMeterCounter;

    private FoxFormPlayer _player;
    protected override void OnAwake()
    {
        base.OnAwake();
        _player = _player.FromScene(true);
    }

    public void Show()
    {
        _player.MoveController.BlockMovement(this);
        _player.JumpController.BlockMovement(this);
        SpriteRenderer.enabled = true;
        Menu.SetActive(true);
        DeathsCounter.ShowText();
        EnemiesKilledCounter.ShowText();
        GhostMeterCounter.ShowText();
    }

    public void Hide()
    {
        SpriteRenderer.enabled = true;
        Menu.SetActive(false);
    }
}
