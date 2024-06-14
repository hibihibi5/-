using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [Header("���Ԑ���")]
    [SerializeField] float _timeRimit = default;
    [Header("���Ԑ����̃e�L�X�g")]
    [SerializeField] Text _timeRimitText = default;

    private void Start()
    {
        _timeRimitText.text = _timeRimit.ToString("F0"); // �L�����o�X�Ɏ��Ԑ����̏����l������
    }

    private void FixedUpdate()
    {
        TimeCount();
    }

    /// <summary>
    /// ���Ԑ����̏���
    /// </summary>
    private void TimeCount()
    {
        if (_timeRimit > 0)
        {
            _timeRimit -= Time.deltaTime;
            _timeRimitText.text = _timeRimit.ToString("F0");
            print("i");
        }
        else
        {
            _timeRimitText.text = "0";
            print("���Ԑ؂�");
            Time.timeScale = 0;
        }
    }
}
