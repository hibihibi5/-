using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using static WallButtonClass;

public class TerminalCon : MonoBehaviour
{
    [Header("�ϐ��̎w��")]
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
    [Tooltip("���e��u���R�X�g")]
    [SerializeField] int _SetBombCost = 0;
    [Tooltip("��d�R�X�g")]
    [SerializeField] int _PowerOutageCost = 0;
    [Tooltip("��d�̃N�[���^�C��")]
    [SerializeField] float _coolTimeOfPowerOut = 0;

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
    [Tooltip("���e�̋����N���{�^��")]
    [SerializeField] Button _explosionButton = default;
    [Tooltip("��d�{�^��")]
    [SerializeField] Button _powerOutButton = default;
    [Tooltip("��d�p�e�擾")]
    [SerializeField] GameObject power;
    [Tooltip("���ʉ��p�擾")]
    [SerializeField] private GameObject seObj;
    [Tooltip("�������擾")]
    [SerializeField] GameObject explan;

    private Vector3 currentPosition = Vector3.zero;
    private int _nebanebaListCount = 0;
    private float _cost = 0; // ���݂̃R�X�g
    private float _coolTimeCount = 0; // ���݂̃N�[���^�C��
    private float _coolTimeCountOfPower = 0; // ���݂̒�d�N�[���^�C��
    private bool _isButtonClick = false; // �{�^�������̔���
    private bool _isNebanebaClick = false;

    // public---------------------------------------------------------------------------------------

    /// <summary>
    /// ���e��u���邩�ǂ����̏���
    /// </summary>
    /// <returns></returns>
    public bool GetBombCost()
    {
        if (_cost >= _SetBombCost)
        {
            _cost -= _SetBombCost;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// �O������̃l�o�ݒu��Ԃ̉���
    /// </summary>
    public void OutCancelNebanebaSet()
    {
        CancelNebanebaSet();
    }

    // private--------------------------------------------------------------------------------------

    private void Awake()
    {
        //EventTrigger�R���|�[�l���g���擾
        EventTrigger eventTrigger = GameObject.FindGameObjectWithTag("EventTrigger").GetComponent<EventTrigger>();

        //�C�x���g�̐ݒ�ɓ���
        EventTrigger.Entry entry = new EventTrigger.Entry();

        //PointerDown(�������u�ԂɎ��s����)�C�x���g�^�C�v��ݒ�
        entry.eventID = EventTriggerType.PointerDown;

        //�֐���ݒ�
        entry.callback.AddListener((x) =>
        {
            NebaSet();
        });

        //�C�x���g�̐ݒ��EventTrigger�ɔ��f
        eventTrigger.triggers.Add(entry);
    }

    private void Start()
    {
        foreach (ButtonClass buttonClass in _buttonClass)
        {
            buttonClass.button.onClick.AddListener(() => WallButton(buttonClass));
        }

        _nebanebaButton.onClick.AddListener(() => NebanebaClick());

        _explosionButton.onClick.AddListener(() => BombExplosion());

        _powerOutButton.onClick.AddListener(() => PowerOutage());
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
        _mainPointText.text = _cost.ToString("F1"); // ���C���L�����o�X
        _terminalPointText.text = _cost.ToString("F1"); // �[���L�����o�X
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

        if (_coolTimeCountOfPower > 0)
        {
            _coolTimeCountOfPower -= Time.deltaTime;
        }
        else
        {
            _powerOutButton.interactable = true;
        }
    }

    /// <summary>
    /// �Ǘp�{�^���̏���
    /// </summary>
    /// <param name="buttonClass"></param>
    private void WallButton(ButtonClass buttonClass)
    {
        CancelNebanebaSet();

        if (!_isButtonClick && (_cost >= _wallMoveCost))
        {
            _coolTimeCount = _coolTime;
            _isButtonClick = true;
            seObj.GetComponent<SEScript>().Wall();
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
        else if(_cost < _wallMoveCost)
        {
            explan.GetComponent<ExplanationScri>().ShutterTarinu();
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
                seObj.GetComponent<SEScript>().Cleck();
                _isNebanebaClick = true;
                _nebanebaButton.interactable = false; // �{�^�����A�N�e�B�u��Ԃ�
            }
        }
        else
        {
            explan.GetComponent<ExplanationScri>().NebanebaTarinu();
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

            _nebanebaObjs[_nebanebaListCount].GetComponent<SphereCollider>().enabled = false;
            _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

            // �E�N���b�N�����Ƃ��ɐݒu��Ԃ�����
            if (Input.GetMouseButtonDown(1))
            {
                CancelNebanebaSet();
            }
        }
    }

    /// <summary>
    /// �l�o�ݒu��Ԃ̉���
    /// </summary>
    private void CancelNebanebaSet()
    {
        if (_isNebanebaClick)
        {
            _isNebanebaClick = false;
            _nebanebaButton.interactable = true; // �{�^�����A�N�e�B�u��Ԃ�
            _nebanebaObjs[_nebanebaListCount].GetComponent<SphereCollider>().enabled = true; // �����蔻����I��
            _nebanebaObjs[_nebanebaListCount].transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// �˂΂˂΂̐ݒu����
    /// </summary>
    private void NebaSet()
    {
        if (_isNebanebaClick && Input.GetMouseButtonDown(0))
        {
            _isNebanebaClick = false;
            _nebanebaButton.interactable = true; // �{�^�����A�N�e�B�u��Ԃ�
            _cost -= _nebanebaCost; // �R�X�g�̏���

            _nebanebaObjs[_nebanebaListCount].GetComponent<NebanebaCon>().enabled = true; // ���ŏ����̊J�n�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@
            _nebanebaObjs[_nebanebaListCount].GetComponent<SphereCollider>().enabled = true; // �����蔻����I�t
            _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

            _nebanebaListCount++;
            if (_nebanebaListCount >= _nebanebaObjs.Length)
            {
                _nebanebaListCount = 0;
            }
        }
    }

    /// <summary>
    /// �S�Ă̔��e�̋����N������
    /// </summary>
    private void BombExplosion()
    {
        CancelNebanebaSet();

        if (GameObject.FindGameObjectsWithTag("Bomb").Length >= 1)
        {
            seObj.GetComponent<SEScript>().Bomb();
        }
        foreach (GameObject bombCon in GameObject.FindGameObjectsWithTag("Bomb"))
        {
            bombCon.GetComponent<BombCon>().CompulsionExplosion();
        }
    }
    /// <summary>
    /// ��d����
    /// </summary>
    private void PowerOutage()
    {
        CancelNebanebaSet();
        if (_cost >= _PowerOutageCost)
        {
            power.GetComponent<PowerOut>().PowerOutage();
            _powerOutButton.interactable = false;
            _coolTimeCountOfPower = _coolTimeOfPowerOut;
            _cost -= _PowerOutageCost;
            seObj.GetComponent<SEScript>().PowerOut();
        }
        else
        {
            explan.GetComponent<ExplanationScri>().PowerOutTarinu();
        }
    }
}
