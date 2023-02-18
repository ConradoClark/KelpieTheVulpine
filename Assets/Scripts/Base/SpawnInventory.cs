using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;

public class SpawnInventory : BaseGameObject
{
    public string Identifier;
    public ScriptPrefab InventoryObject;
    private Collectable _collectable;

    private FollowingInventoryPool _inventoryPool;
    private Player _player;

    protected override void OnAwake()
    {
        base.OnAwake();
        _collectable = GetComponent<Collectable>();
        _inventoryPool = SceneObject<FollowingInventoryPoolManager>.Instance().GetEffect(InventoryObject);
        _player = _player.FromScene();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _collectable.OnPickup += OnPickup;
    }

    private void OnPickup(Collectable obj)
    {
        if (!_inventoryPool.TryGetFromPool(out var inventory)) return;
        inventory.Identifier = Identifier;
        inventory.transform.position = transform.position;
        _player.AddInventory(inventory);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _collectable.OnPickup -= OnPickup;
    }
}
