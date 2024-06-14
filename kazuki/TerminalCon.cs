using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WallButtonClass;

public class TerminalCon : MonoBehaviour
{
    [SerializeField] Animator _testAnime = default;
    [SerializeField] Button _testButton = default;

    [SerializeField] ButtonClass[] _buttonClass = new ButtonClass[4];

    [Header("�R���|�[�l���g�擾")]
    [Tooltip("���C����ʂ̃|�C���g�\���e�L�X�g")]
    [SerializeField] Text _mainPointText = default;
    [Tooltip ("�R�X�g�̒��܂鑬��")]
    [SerializeField] float _PointChargeSpeed = 1;
    [Tooltip ("�R�X�g�̍ő�l")]
    [SerializeField] float _maxPoint = 100;

    Text _terminalPointText = default;

    [SerializeField] private float _point = 0; // ���݂̃|�C���g

    private void Awake()
    {
        _terminalPointText = this.gameObject.transform.Find("PointBackGuround/Point").GetComponent<Text>();
    }

    private void Start()
    {
        foreach (ButtonClass buttonClass in _buttonClass)
        {
            foreach (Animator animator in buttonClass.animator)
            {
                buttonClass.button.onClick.AddListener(() => WallButton(animator));
            }
        }
    }

    private void FixedUpdate()
    {
        _point += _PointChargeSpeed * Time.deltaTime; // ���Ԍo�߂Őݒ肵�����x�ŃR�X�g�����܂�
        _point = Mathf.Clamp(_point, 0, _maxPoint); // �ő�l�ɃN�����v
        _mainPointText.text = _point.ToString("F2"); // ���C���L�����o�X
        _terminalPointText.text = _point.ToString("F2"); // �[���L�����o�X
    }

    private void WallButton(Animator animator)
    {
        if (_point >= 10) // ��
        {
            //foreach (ButtonClass buttonClass in _buttonClass)
            //{
            //    foreach (Animator anime in buttonClass.animator)
            //    {
            //        anime.Play("New State");
            //    }
            //}
            
            _point -= 10;
            animator.SetTrigger("Trigger");
        }
    }
}
