using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelSelector : BaseGameObject
{
    public GameProgress GameProgress;
    public LevelPanel[] Panels;
    private Vector2Int _currentPos;

    public InputActionReference HorizontalMovement;
    public InputActionReference VerticalMovement;
    public InputActionReference Confirm;

    public enum LevelSelectorEvents
    {
        OnLevelSelected
    }

    private IEventPublisher<LevelSelectorEvents, LevelPanel> _panelChanged;
    protected override void OnAwake()
    {
        base.OnAwake();
        _panelChanged = this.RegisterAsEventPublisher<LevelSelectorEvents, LevelPanel>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(Handle());
        _currentPos = GameProgress.LastLevelPlayed == null ? Vector2Int.zero : Panels.FirstOrDefault(p => p.Level == GameProgress.LastLevelPlayed)?.IconPosition ??
                      Vector2Int.zero;
    }

    private IEnumerable<IEnumerable<Action>> Handle()
    {
        while (ComponentEnabled)
        {
            if (HorizontalMovement.action.WasPerformedThisFrame())
            {
                var direction = Mathf.Sign(HorizontalMovement.action.ReadValue<float>());
                var match = new Vector2Int(_currentPos.x + (int)direction, _currentPos.y);

                foreach (var panel in Panels)
                {
                    if (panel.IconPosition != match) continue;
                    _currentPos = match;
                    transform.position = panel.transform.position;
                    _panelChanged.PublishEvent(LevelSelectorEvents.OnLevelSelected, panel);
                    break;
                }

                while (HorizontalMovement.action.WasPerformedThisFrame())
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }

            if (VerticalMovement.action.WasPerformedThisFrame())
            {
                var direction = Mathf.Sign(VerticalMovement.action.ReadValue<float>());
                var match = new Vector2Int(_currentPos.x, _currentPos.y + (int)direction);

                foreach (var panel in Panels)
                {
                    if (panel.IconPosition != match) continue;
                    _currentPos = match;
                    transform.position = panel.transform.position;
                    _panelChanged.PublishEvent(LevelSelectorEvents.OnLevelSelected, panel);
                    break;
                }

                while (VerticalMovement.action.WasPerformedThisFrame())
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }

            if (Confirm.action.WasReleasedThisFrame())
            {
                var levelPanel = Panels.FirstOrDefault(p => p.Selected);
                if (levelPanel != null)
                {
                    GameProgress.LastLevelPlayed = levelPanel.Level;
                    DefaultMachinery.FinalizeWith(() =>
                    {
                        SceneManager.LoadScene(levelPanel.LevelScene, LoadSceneMode.Single);
                    });
                }
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
