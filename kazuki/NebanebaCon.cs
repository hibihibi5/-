using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebanebaCon : MonoBehaviour
{
    [Header("�ϐ��̎w��")]
    [Tooltip("�˂΂˂΂����ł���܂ł̎���")]
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
    /// �˂΂˂΂������鏈��
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
