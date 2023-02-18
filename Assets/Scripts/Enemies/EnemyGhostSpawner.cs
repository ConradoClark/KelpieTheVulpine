using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Effects;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGhostSpawner : BaseGameObject
{
    public ScriptPrefab SpawnEffect;
    public ScriptPrefab[] Ghosts;
    public int MinAmount;
    public int MaxAmount;
    public float ChanceInPercentage;
    public float CheckFrequencyInSeconds;

    public float SpawnRadius;

    private Player _player;
    private ViewportObjectPoolManager _objectPoolManager;
    private PrefabPool _effectPool;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance(true);
        _objectPoolManager = SceneObject<ViewportObjectPoolManager>.Instance(true);
        _effectPool = SceneObject<EffectsManager>.Instance().GetEffect(SpawnEffect);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandleSpawn());
    }

    private IEnumerable<IEnumerable<Action>> HandleSpawn()
    {
        while (ComponentEnabled)
        {
            if (_player.CurrentForm == Player.PlayerForms.Fox)
            {
                // no ghosts in the realm of living
                foreach (var pool in _objectPoolManager.Effects.Values)
                {
                    pool.ReleaseAll();
                }
            }

            while (_player.CurrentForm == Player.PlayerForms.Fox)
            {
                yield return TimeYields.WaitOneFrameX;
            }

            while (_player.CurrentForm == Player.PlayerForms.Ghost)
            {
                yield return TimeYields.WaitSeconds(GameTimer, CheckFrequencyInSeconds);
                if (Random.value > ChanceInPercentage * 0.01f) continue;

                var spawnCount = Random.Range(MinAmount, MaxAmount);

                for (var i = 0; i < spawnCount; i++)
                {
                    var ghostToSpawn = Ghosts[Random.Range(0, Ghosts.Length)];
                    var pos = _player.GhostForm.transform.position
                              + (Vector3)Random.insideUnitCircle.normalized * SpawnRadius
                              + (Vector3)Random.insideUnitCircle * 0.1f;

                    if (_effectPool.TryGetFromPool(out var effect))
                    {
                        effect.Component.transform.position = pos;
                        yield return TimeYields.WaitSeconds(GameTimer, 1.2, breakCondition:
                           () => _player.CurrentForm == Player.PlayerForms.Fox);
                    }

                    if (_player.CurrentForm == Player.PlayerForms.Fox) break;

                    if (!_objectPoolManager.GetEffect(ghostToSpawn).TryGetFromPool(out var ghost)) continue;

                    ghost.UnderlyingObject.transform.position =
                        -Camera.main.transform.position + pos;
                }
            }
        }
    }
}

