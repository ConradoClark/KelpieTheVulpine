using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;

public class LevelEndSign : BaseGameObject
{
    public Collectable Sign;
    private LevelCompleteOverlay _levelCompleteOverlay;

    protected override void OnAwake()
    {
        base.OnAwake();
        _levelCompleteOverlay = _levelCompleteOverlay.FromScene(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Sign.OnPickup += Sign_OnPickup;
    }

    private void Sign_OnPickup(Collectable obj)
    {
        _levelCompleteOverlay.Show();
    }
}
