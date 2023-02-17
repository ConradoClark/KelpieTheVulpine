using Licht.Unity.Objects;
using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Pooling;
using UnityEngine;

public class CameraPosSync : EffectPoolable
{
    public Transform UnderlyingObject;
    public EffectPoolable Reference;
    private void LateUpdate()
    {
        var pos = Camera.main.transform.position;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        if (Reference.IsEffectOver)
        {
            EndEffect();
        }
    }
    public override void OnActivation()
    {
        if (Reference.gameObject.activeSelf) return;

        Reference.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Reference.gameObject.SetActive(false);
    }
}
