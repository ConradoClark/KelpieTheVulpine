using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Unity.CharacterControllers;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FoxFormPlayer FoxForm;
    public PlayerForms CurrentForm;

    [Serializable]
    public enum PlayerForms
    {
        Fox,
        Ghost
    }

    public bool IsFox => CurrentForm == PlayerForms.Fox;
    public bool IsGhost => CurrentForm == PlayerForms.Ghost;
}
