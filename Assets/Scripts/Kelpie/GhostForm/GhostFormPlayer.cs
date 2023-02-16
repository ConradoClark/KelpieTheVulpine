using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Effects;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using Licht.Unity.Physics.Forces;
using Licht.Unity.Pooling;

public class GhostFormPlayer : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public LichtTopDownMoveController MoveController;
    public GhostFollowMouse GhostMouseFollower;
    public LichtPhysicsCollisionDetector BodyTrigger;
    public ScriptPrefab RebirthEffect;
    public TintFlash RebirthFlash;
    public float GhostFormDrainRate;

    private Gravity _gravity;
    private Player _player;
    private IEventPublisher<StateEvents> _eventPublisher;
    private KelpieDeathLoop _deathLoop;
    private DarkWorldController _darkWorldController;
    private PrefabPool _rebirthPool;
    private SpinningLightRays _lightRays;
    private GhostCounter _ghostCounter;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance();
        _eventPublisher = this.RegisterAsEventPublisher<StateEvents>();
        _deathLoop = SceneObject<KelpieDeathLoop>.Instance(true);
        _darkWorldController = SceneObject<DarkWorldController>.Instance();
        _rebirthPool = SceneObject<EffectsManager>.Instance().GetEffect(RebirthEffect);
        _lightRays = SceneObject<SpinningLightRays>.Instance(true);
        _ghostCounter = SceneObject<GhostCounter>.Instance(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(DisableGravityForGhost());
        DefaultMachinery.AddBasicMachine(HandleBodyCollision());
        DefaultMachinery.AddBasicMachine(DrainGhostCounter());
    }

    private IEnumerable<IEnumerable<Action>> DrainGhostCounter()
    {
        while (ComponentEnabled)
        {
            _ghostCounter.Drain(GhostFormDrainRate);
            yield return TimeYields.WaitMilliseconds(GameTimer, 20);
        }
    }

    private IEnumerable<Action> DisableGravityForGhost()
    {
        yield return TimeYields.WaitOneFrame;
        _gravity = SceneObject<Gravity>.Instance();
        _gravity.BlockForceFor(this, PhysicsObject);
    }

    private IEnumerable<IEnumerable<Action>> HandleBodyCollision()
    {
        while (ComponentEnabled)
        {
            if (BodyTrigger.Triggers.Any(t => t.TriggeredHit))
            {
                DefaultMachinery.AddBasicMachine(_darkWorldController.ExitFromDarkWorld());
                _player.CurrentForm = Player.PlayerForms.Fox;
                _eventPublisher.PublishEvent(StateEvents.OnFoxForm);
                _player.FoxForm.transform.position = _player.GhostForm.transform.position;
                _deathLoop.Hide();

                if (_rebirthPool.TryGetFromPool(out var effect))
                {
                    effect.Component.transform.position = _player.FoxForm.transform.position;
                }

                _lightRays.Show(_player.FoxForm.transform.position);

                _player.FoxForm.gameObject.SetActive(true);

                _player.FoxForm.MoveController.BlockMovement(this);
                _player.FoxForm.JumpController.BlockMovement(this);
                _gravity.BlockForceFor(this, _player.FoxForm.PhysicsObject);

                _player.GhostForm.gameObject.SetActive(false);

                yield return RebirthFlash.Flash().AsCoroutine();

                _player.FoxForm.MoveController.UnblockMovement(this);
                _player.FoxForm.JumpController.UnblockMovement(this);
                _gravity.UnblockForceFor(this, _player.FoxForm.PhysicsObject);

                _eventPublisher.PublishEvent(StateEvents.OnInvincibilityFrames);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
