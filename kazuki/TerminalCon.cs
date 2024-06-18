using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static WallButtonClass;

public class TerminalCon : MonoBehaviour
{
    [SerializeField] ButtonClass[] _buttonClass = new ButtonClass[4];

    [Header("variable")]
    [Tooltip ("�R�X�g�̒��܂鑬��")]
    [SerializeField] float _PointChargeSpeed = 1;
    [Tooltip ("�R�X�g�̍ő�l")]
    [SerializeField] float _maxPoint = 100;
    [Tooltip("�ǂ𓮂����R�X�g")]
    [SerializeField] int _wallMoveCost = 0;
    [Tooltip("�{�^���̃N�[���^�C��")]
    [SerializeField] float _coolTime = 0;

    [Header("�R���|�[�l���g�擾")]
    [Tooltip("���C����ʂ̃R�X�g�\���e�L�X�g")]
    [SerializeField] Text _mainPointText = default;
    [Tooltip("�[���̃R�X�g�\���e�L�X�g")]
    [SerializeField] Text _terminalPointText = default;

    private float _cost = 0; // ���݂̃R�X�g
    private float _coolTimeCount = 0; // ���݂̃N�[���^�C��
    private bool _isButtonClick = false; // �{�^�������̔���

    // public---------------------------------------------------------------------------------------



    // private--------------------------------------------------------------------------------------

    private void Start()
    {
        foreach (ButtonClass buttonClass in _buttonClass)
        {
            buttonClass.button.onClick.AddListener(() => WallButton(buttonClass));
        }
    }

    private void FixedUpdate()
    {
        CostDisplayUpdate();

        CoolTimeCount();
    }

    /// <summary>
    /// �R�X�g�̍X�V����
    /// </summary>
    private void CostDisplayUpdate()
    {
        _cost += _PointChargeSpeed * Time.deltaTime; // ���Ԍo�߂Őݒ肵�����x�ŃR�X�g�����܂�
        _cost = Mathf.Clamp(_cost, 0, _maxPoint); // �ő�l�ɃN�����v
        _mainPointText.text = _cost.ToString("F2"); // ���C���L�����o�X
        _terminalPointText.text = _cost.ToString("F2"); // �[���L�����o�X
    }

    /// <summary>
    /// �N�[���^�C���̏���
    /// </summary>
    private void CoolTimeCount()
    {
        if (_coolTimeCount > 0)
        {
            _coolTimeCount -= Time.deltaTime;
        }
        else
        {
            _isButtonClick = false;

            foreach (ButtonClass button in _buttonClass)
            {
                // �{�^���̗L����
                button.button.interactable = true;
            }
        }
    }

    /// <summary>
    /// �Ǘp�{�^���̏���
    /// </summary>
    /// <param name="buttonClass"></param>
    private void WallButton(ButtonClass buttonClass)
    {
        if (!_isButtonClick && (_cost >= _wallMoveCost))
        {
            _coolTimeCount = _coolTime;
            _isButtonClick = true;

            _cost -= _wallMoveCost;

            bool isAnimeBool = buttonClass.animator[0].GetBool("WallBool");

            foreach (ButtonClass button in _buttonClass)
            {
                // �{�^���̖�����
                button.button.interactable = false;

                // �S�Ă̔���߂�
                foreach (Animator anime in button.animator)
                {
                    anime.SetBool("WallBool", false);
                }
            }

            foreach (Animator animator in buttonClass.animator)
            {
                animator.SetBool("WallBool", !isAnimeBool);
            }
        }
    }
}
