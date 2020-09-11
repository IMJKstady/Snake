using Snake;
using Snake.UI;
using UnityEngine;

namespace Ui.Mediator
{
    public class PlayWithBotMediator : UiMediator<PlayWithBot>
    {
        [Inject]
        public StartGameSignal startPvBSignal { get; set; }
        [Inject]
        public AudioManager audioManager { get; set; }
        
        public override void Init()
        {
            view.selectDifficulty.AddListener(SelectDifficulty);
            view.back.onClick.AddListener(Back);
        }
        
        private void SelectDifficulty(int code)
        {
            PlayerPrefs.SetInt("aiLevel", code);
            startPvBSignal.Dispatch(GameMode.PvB);
            audioManager.PlayButtonEffect();
            Close();
        }

        public override void Back()
        {
            base.Back();
            audioManager.PlayButtonEffect();
        }
    }
}