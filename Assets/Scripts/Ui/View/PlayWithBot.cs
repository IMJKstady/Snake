using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace Snake.UI
{
    // 人机对战
    public class PlayWithBot : ViewBase
    {
        [SerializeField]
        private Button easy;
        [SerializeField]
        private Button medium;
        [SerializeField]
        private Button hard;

        public Signal<int> selectDifficulty = new Signal<int>();
        public Button back;
        public override void Init()
        {
            easy.onClick.AddListener(() => { selectDifficulty.Dispatch(1);});
            medium.onClick.AddListener(() => { selectDifficulty.Dispatch(2); });
            hard.onClick.AddListener(() => { selectDifficulty.Dispatch(3); });
        }
    }
}