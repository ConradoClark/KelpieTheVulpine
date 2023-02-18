using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.UI;
using UnityEngine;

public class MainMenu_Options : UIAction
{
    public SpriteRenderer Button;
    public Color UnselectedColor;

    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        yield break;
    }

    public override void OnSelect(bool manual)
    {
        base.OnSelect(manual);
        Button.color = Color.white;
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        Button.color = UnselectedColor;
    }

    public override void OnInit()
    {
    }
}
