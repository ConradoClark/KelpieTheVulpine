using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;

public class GameOverOnDepletedGhostMeter : BaseGameObject
{
    public TintFlash DeathFlash;
    public ScriptPrefab DeathEffect;
    private GameOverOverlay _uiOverlay;
    private GhostCounter _counter;
    private GhostFormPlayer _player;
    private GameOverVolumeController _volumeController;
    private PrefabPool _deathEffectPool;
    private bool _died;
    protected override void OnAwake()
    {
        base.OnAwake();
        _counter = _counter.FromScene();
        _player = _player.FromScene(true);
        _uiOverlay = _uiOverlay.FromScene(true);
        _volumeController = _volumeController.FromScene(true);
        _deathEffectPool = this.GetObjectPool(DeathEffect);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _counter.OnValueChange += OnValueChanged;
    }

    private void OnValueChanged(float obj)
    {
        if (_died || !(obj <= _counter.MinValue)) return;

        _died = true;
        DefaultMachinery.AddBasicMachine(Death());
    }

    private IEnumerable<IEnumerable<Action>> Death()
    {
        _player.MoveController.BlockMovement(this);
        _player.GhostMouseFollower.BlockMovement(this);
        if (_deathEffectPool.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = _player.transform.position;
        }

        DefaultMachinery.AddBasicMachine(_volumeController.GameOver());
        DefaultMachinery.AddBasicMachine(DeathFlash.Flash());
        yield return TimeYields.WaitSeconds(GameTimer, 1);
        _player.gameObject.SetActive(false);
        _uiOverlay.Show();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _counter.OnValueChange -= OnValueChanged;
    }
}
