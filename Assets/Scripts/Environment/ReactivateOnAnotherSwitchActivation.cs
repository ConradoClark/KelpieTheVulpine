using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;

public class ReactivateOnAnotherSwitchActivation : BaseGameObject
{
    public SwitchObject Target;
    private SwitchObject _self;

    protected override void OnAwake()
    {
        base.OnAwake();
        _self = GetComponent<SwitchObject>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Target.OnActivation += Target_OnActivation;
    }

    private void Target_OnActivation()
    {
        _self.ResetSwitch();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Target.OnActivation -= Target_OnActivation;
    }
}
