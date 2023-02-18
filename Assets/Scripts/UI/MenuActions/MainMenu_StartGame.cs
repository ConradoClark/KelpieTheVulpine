using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_StartGame : UIAction
{
    public SpriteRenderer Button;
    public Color UnselectedColor;

    [Scene]
    public string Scene;
    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        DefaultMachinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene(Scene, LoadSceneMode.Single);
        });

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
