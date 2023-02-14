using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0,0, Random.value * 360));
    }
}
