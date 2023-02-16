using Licht.Impl.Events;
using Licht.Unity.Effects;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropGhostParticlesOnDeath : BaseGameObject
{
    public int MinParticles;
    public int MaxParticles;

    public ScriptPrefab GhostParticle;
    public Killable Killable;

    private PrefabPool _ghostParticlePool;
    private Camera _gameCamera;
    private Camera _uiCamera;

    protected override void OnAwake()
    {
        base.OnAwake();
        _ghostParticlePool = SceneObject<EffectsManager>.Instance(true).GetEffect(GhostParticle);
        _uiCamera = SceneObject<UICamera>.Instance(true).Camera;
        _gameCamera = Camera.main;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
    }

    private void OnDeath(Killable obj)
    {
        if (obj != Killable) return;

        if (!_ghostParticlePool.TryGetManyFromPool(Random.Range(MinParticles, MaxParticles),
                out var effects)) return;


        var pos = _uiCamera.ViewportToWorldPoint(
            _gameCamera.WorldToViewportPoint(Killable.transform.position));
        foreach (var effect in effects)
        {
            effect.Component.transform.position = new Vector3(pos.x, pos.y,
                effect.Component.transform.position.z);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<StateEvents, Killable>(StateEvents.OnDeath, OnDeath);
    }
}
