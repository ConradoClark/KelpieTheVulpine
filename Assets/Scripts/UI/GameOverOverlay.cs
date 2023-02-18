using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

public class GameOverOverlay : BaseGameObject
{
    public SpriteRenderer SpriteRenderer;

    public void Show()
    {
        SpriteRenderer.enabled = true;
    }

    public void Hide()
    {
        SpriteRenderer.enabled = true;
    }
}
