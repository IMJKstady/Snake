using System;
using UnityEngine;
using UnityEngine.UI;

namespace Snake.UI
{
    // 游戏模式选择
    public class SelectGame : ViewBase
    {
        public Button one;
        public Button two;
        public Button online;
        public Button back;

        [Space]
        public ViewBase oneSkip;
        public ViewBase onlineSkip;
    }
}