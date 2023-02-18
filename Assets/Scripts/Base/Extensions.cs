using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
