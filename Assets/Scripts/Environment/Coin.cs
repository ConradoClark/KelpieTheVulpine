using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class Coin : BaseGameObject
{
    public Vector3 PickupOffset;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer SpriteOutline;
    public LichtPhysicsCollisionDetector CollisionDetector;
    public ScriptPrefab PickupEffect;
    private IEventPublisher<CoinEvents> _pickupEvent;

    private bool _pickedUp;

    protected override void OnAwake()
    {
        base.OnAwake();
        _pickupEvent = this.RegisterAsEventPublisher<CoinEvents>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(HandlePickup());
    }

    public enum CoinEvents
    {
        OnPickup
    }

    private IEnumerable<IEnumerable<Action>> HandlePickup()
    {
        while (!_pickedUp && ComponentEnabled)
        {
            if (CollisionDetector.Triggers.Any(t => t.TriggeredHit))
            {
                if (this.GetObjectPool(PickupEffect).TryGetFromPool(out var effect))
                {
                    effect.Component.transform.position = transform.position + PickupOffset;
                }
                _pickupEvent.PublishEvent(CoinEvents.OnPickup);
                _pickedUp = true;
            }

            yield return TimeYields.WaitOneFrameX;
        }

        SpriteRenderer.enabled = false;
        SpriteOutline.enabled = true;
    }
}
