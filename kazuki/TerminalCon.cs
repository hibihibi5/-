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

    [Header("コンポーネント取得")]
    [Tooltip("メイン画面のポイント表示テキスト")]
    [SerializeField] Text _mainPointText = default;
    [Tooltip ("コストの貯まる速さ")]
    [SerializeField] float _PointChargeSpeed = 1;
    [Tooltip ("コストの最大値")]
    [SerializeField] float _maxPoint = 100;

    Text _terminalPointText = default;

    [SerializeField] private float _point = 0; // 現在のポイント

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
        _point += _PointChargeSpeed * Time.deltaTime; // 時間経過で設定した速度でコストが貯まる
        _point = Mathf.Clamp(_point, 0, _maxPoint); // 最大値にクランプ
        _mainPointText.text = _point.ToString("F2"); // メインキャンバス
        _terminalPointText.text = _point.ToString("F2"); // 端末キャンバス
    }

    private void WallButton(Animator animator)
    {
        if (_point >= 10) // 仮
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
