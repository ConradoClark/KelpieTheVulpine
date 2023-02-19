using Licht.Impl.Events;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class LevelPanel : BaseGameObject
{
    public SpriteRenderer Panel;
    public GameProgress GameProgress;

    public ScriptIdentifier Level;

    [Scene]
    public string LevelScene;
    public string Description;
    public ScriptIdentifier[] RequiredLevelsCompleted;
    public ScriptIdentifier[] RequiredAchievements;

    public string LockedMessage;

    public Vector2Int IconPosition;
    public Color TextColor;

    public bool IsLocked { get; private set; }
    public bool Selected;

    
    private LevelSelectText _levelSelectText;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.ObserveEvent<LevelSelector.LevelSelectorEvents, LevelPanel> (
            LevelSelector.LevelSelectorEvents.OnLevelSelected, OnLevelSelection);
    }

    private void OnLevelSelection(LevelPanel obj)
    {
        Selected = obj == this;
        SetText();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopObservingEvent<LevelSelector.LevelSelectorEvents, LevelPanel>(
            LevelSelector.LevelSelectorEvents.OnLevelSelected, OnLevelSelection);
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        _levelSelectText = _levelSelectText.FromScene(true);

        foreach (var level in RequiredLevelsCompleted)
        {
            if (GameProgress.IsLevelCompleted(level.Name)) continue;
            IsLocked = true;
            break;
        }

        if (!IsLocked)
        {
            foreach (var id in RequiredAchievements)
            {
                if (GameProgress.HasAchievement(id.Name)) continue;
                IsLocked = true;
                break;
            }
        }

        SetText();
    }

    public void SetText()
    {
        if (IsLocked)
        {
            if (Selected)
            {
                _levelSelectText.TextComponent.text = LockedMessage;
                _levelSelectText.TextComponent.color = GameProgress.LockedLevelColor;
            }

            Panel.color = GameProgress.LockedLevelPanelColor;
        }
        else
        {
            if (Selected)
            {
                _levelSelectText.TextComponent.text = Description;
                _levelSelectText.TextComponent.color = TextColor;
            }

            Panel.color = Color.white;
        }
    }
}
