using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCon : MonoBehaviour
{
    [Header("�ϐ��̎w��")]
    [Tooltip("��������܂ł̎���")]
    [SerializeField] float _explosionTime = default;
    [Tooltip("�����������̍ő�T�C�Y")]
    [SerializeField] float _maxExplosionSize = default;
    [Tooltip("�����������̑���")]
    [SerializeField] float _explosionSpeed = default;

    private float _explosionTimeNow = 0; // ���݂̔�������܂ł̎���

    // public---------------------------------------------------------------------------------------------------

    /// <summary>
    /// �����N������
    /// </summary>
    public void CompulsionExplosion()
    {
        _explosionTimeNow = 0;
    }

    // private--------------------------------------------------------------------------------------------------

    private void Start()
    {
        _explosionTimeNow = _explosionTime;
    }

    private void FixedUpdate()
    {
        ExplosionCon();
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void ExplosionCon()
    {
        _explosionTimeNow -= Time.deltaTime;

        if (_explosionTimeNow <= 0)
        {
            if (this.gameObject.transform.localScale.x < _maxExplosionSize)
            {
                this.gameObject.transform.localScale = this.gameObject.transform.localScale * _explosionSpeed;
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
