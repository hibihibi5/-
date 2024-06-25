using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebanebaCon : MonoBehaviour
{
    [Header("•Ï”‚Ìw’è")]
    [Tooltip("‚Ë‚Î‚Ë‚Î‚ªÁ–Å‚·‚é‚Ü‚Å‚ÌŠÔ")]
    [SerializeField] float _extinctionTime = 0f;

    private float _extinctionTimeNow = 0;

    private void Awake()
    {
        _extinctionTimeNow = _extinctionTime;
    }

    private void FixedUpdate()
    {
        ExtinctionCount();
    }

    /// <summary>
    /// ‚Ë‚Î‚Ë‚Î‚ªÁ‚¦‚éˆ—
    /// </summary>
    private void ExtinctionCount()
    {
        _extinctionTimeNow -= Time.deltaTime;

        if (_extinctionTimeNow <= 0)
        {
            _extinctionTimeNow = _extinctionTime;
            this.gameObject.transform.position = Vector3.zero;
            this.gameObject.GetComponent<NebanebaCon>().enabled = false;
        }
    }
}
