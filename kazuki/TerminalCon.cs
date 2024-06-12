using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalCon : MonoBehaviour
{
    [SerializeField] Animator _testAnime = default;
    [SerializeField] Button _testButton = default;

    private float _point; // Œ»Ý‚Ìƒ|ƒCƒ“ƒg

    private void Start()
    {
        _testButton.onClick.AddListener(() =>
        {
            _testAnime.SetTrigger("Trigger");
        });
    }
}
