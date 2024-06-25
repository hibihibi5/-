using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCon : MonoBehaviour
{
    [Header("変数の指定")]
    [Tooltip("爆発するまでの時間")]
    [SerializeField] float _explosionTime = default;
    [Tooltip("爆発した時の最大サイズ")]
    [SerializeField] float _maxExplosionSize = default;
    [Tooltip("爆発した時の速さ")]
    [SerializeField] float _explosionSpeed = default;

    private float _explosionTimeNow = 0; // 現在の爆発するまでの時間

    // public---------------------------------------------------------------------------------------------------

    /// <summary>
    /// 強制起爆処理
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
    /// 爆発処理
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
