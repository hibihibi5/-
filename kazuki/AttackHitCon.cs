using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitCon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
    }
}
