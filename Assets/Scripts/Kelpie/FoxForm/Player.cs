using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.CharacterControllers;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FoxFormPlayer FoxForm;
    public GhostFormPlayer GhostForm;
    public PlayerForms CurrentForm;
    private List<FollowingInventory> _inventory;

    [Serializable]
    public enum PlayerForms
    {
        Fox,
        Ghost
    }

    public int InventoryIndexOf(FollowingInventory inventory)
    {
        _inventory ??= new List<FollowingInventory>();
        return _inventory.IndexOf(inventory);
    }

    public void AddInventory(FollowingInventory inventory)
    {
        _inventory ??= new List<FollowingInventory>();
        _inventory.Add(inventory);
    }

    public bool TryGetFromInventory(string identifier, out FollowingInventory inventory)
    {
        if (_inventory == null)
        {
            inventory = null;
            return false;
        }

        inventory = _inventory.FirstOrDefault(item => item.Identifier == identifier);
        return inventory != null;
    }

    public void RemoveFromInventory(FollowingInventory inventory)
    {
        _inventory ??= new List<FollowingInventory>();
        if (_inventory.Contains(inventory))
        {
            _inventory.Remove(inventory);
        }
    }

    public bool HasInInventory(string identifier)
    {
        return _inventory != null && _inventory.Any(item => item.Identifier == identifier);
    }

    public Vector3 CurrentFormPosition => IsFox ? FoxForm.transform.position : GhostForm.transform.position;    

    public bool IsFox => CurrentForm == PlayerForms.Fox;
    public bool IsGhost => CurrentForm == PlayerForms.Ghost;
}
