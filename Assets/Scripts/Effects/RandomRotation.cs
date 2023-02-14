using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(UnityEngine.Random.insideUnitCircle * 360);
    }
}
