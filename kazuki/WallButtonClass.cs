using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallButtonClass : ScriptableObject
{
    [System.Serializable]
    public class ButtonClass
    {
        public Button button = default;
        public Animator[] animator = new Animator[1];
    }
}
