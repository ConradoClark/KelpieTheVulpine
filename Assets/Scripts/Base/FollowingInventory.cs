using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class FollowingInventory : EffectPoolable
{
    public AudioClip SoundOnUse;
    public string Identifier;
    public bool Triggered;
    private Vector2 _refSpeed;
    private Player _player;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = _player.FromScene(true);
    }

    public override void OnActivation()
    {
        DefaultMachinery.AddBasicMachine(FollowPlayer());
    }

    private IEnumerable<IEnumerable<Action>> FollowPlayer()
    {
        while (!IsEffectOver)
        {
            if (!Triggered)
            {
                var targetPos = _player.CurrentForm == Player.PlayerForms.Fox
                    ? _player.FoxForm.transform.position
                    : _player.GhostForm.transform.position;

                var index = _player.InventoryIndexOf(this);
                if (Vector2.Distance(targetPos, transform.position) < 0.4f * (index+1))
                {
                    targetPos = transform.position;
                }

                transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref _refSpeed, 2f,
                    2, (float)GameTimer.UpdatedTimeInMilliseconds * 0.01f);
            }
            

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
