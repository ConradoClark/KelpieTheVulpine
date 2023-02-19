using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

[CreateAssetMenu(fileName = "InputAction", menuName = "Kelpie/GameProgress", order = 1)]
public class GameProgress : ScriptableObject
{
    public ScriptIdentifier LastLevelPlayed;
    public Color LockedLevelColor;
    public Color LockedLevelPanelColor;
    public ScriptIdentifier[] LevelNames;
    public ScriptIdentifier[] Achievements;
    private Dictionary<string, bool> _levelsCompleted;
    private List<string> _achievements;

    public void Awake()
    {
        _levelsCompleted ??= new Dictionary<string, bool>();
        _achievements ??= new List<string>();

        if (LevelNames != null)
        {
            foreach (var id in LevelNames)
            {
                var result = PlayerPrefs.GetString(id.Name, "");
                if (result == "true") _levelsCompleted[id.Name] = true;
            }
        }

        if (Achievements != null)
        {
            foreach (var id in Achievements)
            {
                var result = PlayerPrefs.GetString(id.Name, "");
                if (result == "true" && !_achievements.Contains(id.Name)) _achievements.Add(id.Name);
            }
        }
    }

    public void AddAchievement(string achievement)
    {
        Awake();
        if (!_achievements.Contains(achievement)) _achievements.Add(achievement);
        PlayerPrefs.SetString(achievement,"true");
    }

    public void CompleteLevel(string level)
    {
        Awake();
        _levelsCompleted[level] = true;
        PlayerPrefs.SetString(level, "true");
        PlayerPrefs.Save();
    }

    public bool IsLevelCompleted(string level)
    {
        Awake();
        _levelsCompleted ??= new Dictionary<string, bool>();
        return _levelsCompleted.ContainsKey(level) && _levelsCompleted[level];
    }

    public bool HasAchievement(string achievement)
    {
        Awake();
        return _achievements.Contains(achievement);
    }
}
