using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Pooling;
using Licht.Unity.Effects;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public static class Extensions
{
    public static PrefabPool GetObjectPool(this MonoBehaviour obj, ScriptPrefab prefab)
    {
        return SceneObject<EffectsManager>.Instance(true).GetEffect(prefab);
    }

    public static bool TrySpawnEffect(this ScriptPrefab prefab, Vector3 position, out IPoolableComponent effect)
    {
        var result = SceneObject<EffectsManager>.Instance(true)
            .GetEffect(prefab)
            .TryGetFromPool(out effect);

        if (result)
        {
            effect.Component.transform.position = position;
        }

        return result;
    }

    public static T FromScene<T>(this T obj, bool includeInactive = false) where T: MonoBehaviour
    {

        return SceneObject<T>.Instance(includeInactive);
    }
}
