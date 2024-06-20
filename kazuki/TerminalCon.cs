using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static WallButtonClass;

public class TerminalCon : MonoBehaviour
{
    [Header("variable")]
    [Tooltip ("�R�X�g�̒��܂鑬��")]
    [SerializeField] float _PointChargeSpeed = 1;
    [Tooltip ("�R�X�g�̍ő�l")]
    [SerializeField] float _maxPoint = 999;
    [Tooltip("�ǂ𓮂����R�X�g")]
    [SerializeField] int _wallMoveCost = 0;
    [Tooltip("�{�^���̃N�[���^�C��")]
    [SerializeField] float _coolTime = 0;
    [Tooltip("�˂΂˂΂�u���R�X�g")]
    [SerializeField] int _nebanebaCost = 0;
    [Tooltip("�˂΂˂΃I�u�W�F�N�g�̍���")]
    [SerializeField] float _nebanebaObjHeight = 0;

    [Header("�R���|�[�l���g�擾")]
    [Tooltip("�ǂ𓮂����{�^��s")]
    [SerializeField] ButtonClass[] _buttonClass = new ButtonClass[4];
    [Tooltip("���C����ʂ̃R�X�g�\���e�L�X�g")]
    [SerializeField] Text _mainPointText = default;
    [Tooltip("�[���̃R�X�g�\���e�L�X�g")]
    [SerializeField] Text _terminalPointText = default;
    [Tooltip("�[���J����")]
    [SerializeField] Camera _terminalCamera = default;
    [Tooltip("�˂΂˂ΑI���{�^��")]
    [SerializeField] Button _nebanebaButton;
    [Tooltip("�˂΂˂΃I�u�W�F�N�g�̎擾")]
    [SerializeField] GameObject[] _nebanebaObjs = new GameObject[1];

    private Vector3 currentPosition = Vector3.zero;
    private int _nebanebaListCount = 0;
    private float _cost = 0; // ���݂̃R�X�g
    private float _coolTimeCount = 0; // ���݂̃N�[���^�C��
    private bool _isButtonClick = false; // �{�^�������̔���
    private bool _isNebanebaClick = false;

    // public---------------------------------------------------------------------------------------



    // private--------------------------------------------------------------------------------------

    private void Start()
    {
        foreach (ButtonClass buttonClass in _buttonClass)
        {
            buttonClass.button.onClick.AddListener(() => WallButton(buttonClass));
        }

        _nebanebaButton.onClick.AddListener(() => NebanebaClick());
    }

    private void Update()
    {
        NebanebaCon();
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

    /// <summary>
    /// �˂΂˂΂̑I������
    /// </summary>
    private void NebanebaClick()
    {
        if (_cost >= _nebanebaCost)
        {
            if (!_isNebanebaClick)
            {
                _isNebanebaClick = true;
            }
            else
            {
                _isNebanebaClick = false;
            }
        }
    }

    /// <summary>
    /// �˂΂˂΂̔z�u����
    /// </summary>
    private void NebanebaCon()
    {
        if (_isNebanebaClick)
        {
            var ray = _terminalCamera.ScreenPointToRay(Input.mousePosition);
            var raycastHitList = Physics.RaycastAll(ray).ToList();
            if (raycastHitList.Any())
            {
                var distance = Vector3.Distance(_terminalCamera.transform.position, raycastHitList.First().point);
                var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);

                currentPosition = _terminalCamera.ScreenToWorldPoint(mousePosition);
                currentPosition.y = _nebanebaObjHeight;
            }

            _nebanebaObjs[_nebanebaListCount].GetComponent<MeshCollider>().enabled = false;
            _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

            if (Input.GetMouseButtonDown(1))
            {
                _cost -= _nebanebaCost;
                _isNebanebaClick = false;

                _nebanebaObjs[_nebanebaListCount].GetComponent<NebanebaCon>().enabled = true;
                _nebanebaObjs[_nebanebaListCount].GetComponent<MeshCollider>().enabled = true;
                _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

                _nebanebaListCount++;
                if (_nebanebaListCount >= _nebanebaObjs.Length)
                {
                    _nebanebaListCount = 0;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                _isNebanebaClick = false;
                _nebanebaObjs[_nebanebaListCount].GetComponent<MeshCollider>().enabled = true;
                _nebanebaObjs[_nebanebaListCount].transform.position = Vector3.forward * 100;
            }
        }
    }
}
