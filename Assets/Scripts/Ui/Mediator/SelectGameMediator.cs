using Snake;
using Snake.Online;
using Snake.UI;
using UnityEngine.SceneManagement;

namespace Ui.Mediator
{
    public class SelectGameMediator : UiMediator<SelectGame>
    {
        [Inject]
        public StartGameSignal gameSignal { get; set; }
        [Inject]
        public AudioManager audioManager { get; set; }
        [Inject]
        public OnlineManager onlineManager { get; set; }
        
        public override void Init()
        {
            view.one.onClick.AddListener(One);
            view.two.onClick.AddListener(Two);
            view.online.onClick.AddListener(Online);
            view.back.onClick.AddListener(Back);
            Refresh();
        }

        private void One()
        {
            audioManager.PlayButtonEffect();
            Open(view.oneSkip.id);
        }
        
        private void Two()
        {
            gameSignal.Dispatch(GameMode.PvP);
            audioManager.PlayButtonEffect();
            Close();
        }
        
        private void Online()
        {
            audioManager.PlayButtonEffect();
            onlineManager.Connect(b =>
            {
                if (b)
                {
                    Open(view.onlineSkip.id);
                }
            });
        }

        public override void Back()
        {
            base.Back();
            audioManager.PlayButtonEffect();
        }
    }
}